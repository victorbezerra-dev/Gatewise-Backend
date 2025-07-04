package com.gatewise.keycloak.university;

import java.io.IOException;
import java.util.Objects;

import javax.sql.DataSource;

import org.keycloak.authentication.AuthenticationFlowContext;
import org.keycloak.authentication.AuthenticationFlowError;
import org.keycloak.authentication.Authenticator;
import org.keycloak.authentication.authenticators.browser.UsernamePasswordForm;
import org.keycloak.models.RealmModel;
import org.keycloak.models.UserModel;

import com.gatewise.keycloak.university.dto.AuthResponseDTO;
import com.gatewise.keycloak.university.dto.AuthResponseDTO.AffiliationType;
import com.gatewise.keycloak.university.dto.AuthResponseDTO.Vinculo;
import com.gatewise.keycloak.university.repositories.CustomExternalApi;
import com.gatewise.keycloak.university.repositories.OutboxRepository;
import com.gatewise.keycloak.university.utils.DataSourceProvider;
import com.gatewise.keycloak.university.utils.UserEventBuilder;

import jakarta.ws.rs.core.MultivaluedMap;
import jakarta.ws.rs.core.Response;

public class CustomAuthenticator extends UsernamePasswordForm implements Authenticator {

    public static final String CUSTOM_AUTHENTICATOR_PROVIDER_ID = "university-idp";

    @Override
    public void authenticate(AuthenticationFlowContext context) {
        Response challenge = challenge(context, null, null);
        context.challenge(challenge);
    }

    @Override
    public void action(AuthenticationFlowContext context) {
        MultivaluedMap<String, String> formData = context.getHttpRequest().getDecodedFormParameters();
        String username = formData.getFirst("username");
        String password = formData.getFirst("password");

        if (username == null || password == null) {
            context.failureChallenge(
                    AuthenticationFlowError.INVALID_USER,
                    context.form().setError("Usuário ou senha não informados!").createLoginUsernamePassword()
            );
            return;
        }

        RealmModel realm = context.getRealm();
        UserModel user = context.getSession().users().getUserByUsername(realm, username);

        if (user != null && user.hasRole(realm.getRole("admin"))) {
            if (validatePassword(context, user, formData, false)) {
                context.setUser(user);
                context.success();
                return;
            } else {
                context.failureChallenge(
                        AuthenticationFlowError.INVALID_CREDENTIALS,
                        context.form().setError("Senha inválida para admin!").createLoginUsernamePassword()
                );
                return;
            }
        }

        CustomExternalApi externalApi = new CustomExternalApi();
        AuthResponseDTO response;
        try {
            response = externalApi.loginAndGetUserInfo(username, password);
        } catch (IOException e) {
            context.failureChallenge(
                    AuthenticationFlowError.INTERNAL_ERROR,
                    context.form().setError("Erro interno ao validar usuário!").createLoginUsernamePassword()
            );
            return;
        }

        if (response == null || !response.isAuthenticated()) {
            context.failureChallenge(
                    AuthenticationFlowError.INVALID_CREDENTIALS,
                    context.form().setError("Usuário ou senha inválidos!").createLoginUsernamePassword()
            );
            return;
        }

        Vinculo userData = response.getVinculo();
        String fullName = null;

        if (user == null) {
            user = context.getSession().users().addUser(realm, username);
            user.setEnabled(true);

            String firstName = "";
            String lastName = "";

            if (response.getNome() != null) {
                fullName = response.getNome().trim();
            }

            if (fullName != null && !fullName.isEmpty()) {
                String[] parts = fullName.split("\\s+");
                firstName = parts[0];
                if (parts.length > 1) {
                    lastName = String.join(" ", java.util.Arrays.copyOfRange(parts, 1, parts.length));
                }
            }

            user.setFirstName(firstName);
            user.setLastName(lastName);

            String email = "";
            if (userData != null && userData.getEmail() != null) {
                email = userData.getEmail();
            }
            user.setEmail(email);
            user.setUsername(username);
        }

        String json = UserEventBuilder.buildUserLoggedInEvent(user);
        RabbitMQPublisher publisher = new RabbitMQPublisher();

        DataSource dataSource = DataSourceProvider.get();
        OutboxRepository outbox = new OutboxRepository(dataSource);

        boolean published = publisher.tryPublish(json);
        if (!published) {
            outbox.save("USER_LOGGED_IN", json);
        }

        context.setUser(user);

        if (userData != null) {
            int tipo = userData.getTipo();
            switch (tipo) {
                case AffiliationType.PROFESSOR:
                case AffiliationType.SERVICE_PROVIDER_PROFESSOR:
                    user.grantRole(realm.getRole("role_professor"));
                    break;
                case AffiliationType.STUDENT:
                    user.grantRole(realm.getRole("role_student"));
                    break;
                case AffiliationType.VISITOR:
                    user.grantRole(realm.getRole("role_visitor"));
                    break;
                default:
                    break;
            }
            user.setSingleAttribute("custom.name", Objects.toString(fullName, ""));
            user.setSingleAttribute("custom.description", Objects.toString(userData.getDescricao(), ""));
            user.setSingleAttribute("custom.registration", Objects.toString(userData.getMatricula(), ""));
            user.setSingleAttribute("custom.email", Objects.toString(userData.getEmail(), ""));
            user.setSingleAttribute("custom.photo", Objects.toString(userData.getFoto(), ""));
            user.setSingleAttribute("custom.unitId", String.valueOf(userData.getUnidadeId()));
            user.setSingleAttribute("custom.unitName", Objects.toString(userData.getNomeUnidade(), ""));
            user.setSingleAttribute("custom.courseId", String.valueOf(userData.getCursoId()));
            user.setSingleAttribute("custom.entryYear", String.valueOf(userData.getAnoIngresso()));
            user.setSingleAttribute("custom.statusDescription", Objects.toString(userData.getDescricaoSituacao(), ""));
            user.setSingleAttribute("custom.userType", Objects.toString(userData.getTipo(), ""));
        }

        context.success();
    }
}
