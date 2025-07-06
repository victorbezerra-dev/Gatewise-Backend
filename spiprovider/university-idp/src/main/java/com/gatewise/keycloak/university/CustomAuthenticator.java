package com.gatewise.keycloak.university;

import java.io.IOException;
import java.util.Arrays;
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

public class CustomAuthenticator extends UsernamePasswordForm implements Authenticator {

    public static final String CUSTOM_AUTHENTICATOR_PROVIDER_ID = "university-idp";

    @Override
    public void authenticate(AuthenticationFlowContext context) {
        context.challenge(challenge(context, null, null));
    }

    @Override
    public void action(AuthenticationFlowContext context) {
        MultivaluedMap<String, String> formData = context.getHttpRequest().getDecodedFormParameters();
        String username = formData.getFirst("username");
        String password = formData.getFirst("password");

        if (username == null || password == null) {
            fail(context, AuthenticationFlowError.INVALID_USER, "Usuário ou senha não informados!");
            return;
        }

        RealmModel realm = context.getRealm();
        UserModel user = context.getSession().users().getUserByUsername(realm, username);

        if (user != null && user.hasRole(realm.getRole("admin"))) {
            if (validatePassword(context, user, formData, false)) {
                succeed(context, user);
            } else {
                fail(context, AuthenticationFlowError.INVALID_CREDENTIALS, "Senha inválida para admin!");
            }
            return;
        }

        AuthResponseDTO response = tryAuthenticateExternal(context, username, password);
        if (response == null || !response.isAuthenticated()) return;

        if (user == null) {
            user = createUser(context, realm, username, response);
        }

        assignAttributes(user, response);
        assignRoles(user, context.getRealm(), response.getVinculo());

        tryPublishOrOutbox(user, realm);

        succeed(context, user);
    }

    private void fail(AuthenticationFlowContext context, AuthenticationFlowError error, String message) {
        context.failureChallenge(error, context.form().setError(message).createLoginUsernamePassword());
    }

    private void succeed(AuthenticationFlowContext context, UserModel user) {
        context.setUser(user);
        context.success();
    }

    private AuthResponseDTO tryAuthenticateExternal(AuthenticationFlowContext context, String username, String password) {
        try {
            return new CustomExternalApi().loginAndGetUserInfo(username, password);
        } catch (IOException e) {
            fail(context, AuthenticationFlowError.INTERNAL_ERROR, "Erro interno ao validar usuário!");
            return null;
        }
    }

    private UserModel createUser(AuthenticationFlowContext context, RealmModel realm, String username, AuthResponseDTO response) {
        UserModel user = context.getSession().users().addUser(realm, username);
        user.setEnabled(true);

        String fullName = response.getNome() != null ? response.getNome().trim() : "";
        String[] parts = fullName.split("\\s+");

        user.setFirstName(parts.length > 0 ? parts[0] : "");
        user.setLastName(parts.length > 1 ? String.join(" ", Arrays.copyOfRange(parts, 1, parts.length)) : "");

        String email = response.getVinculo() != null ? Objects.toString(response.getVinculo().getEmail(), "") : "";

        user.setEmail(email);
        user.setUsername(username);

        return user;
    }

    private void assignRoles(UserModel user, RealmModel realm, Vinculo vinculo) {
        switch (vinculo.getTipo()) {
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

    }

    private void assignAttributes(UserModel user, AuthResponseDTO response) {
        Vinculo vinculo = response.getVinculo();
        String fullName = Objects.toString(response.getNome(), "").trim();
        user.setSingleAttribute("custom.name", fullName);
        user.setSingleAttribute("custom.description", Objects.toString(vinculo.getDescricao(), ""));
        user.setSingleAttribute("custom.registration", Objects.toString(vinculo.getMatricula(), ""));
        user.setSingleAttribute("custom.email", Objects.toString(vinculo.getEmail(), ""));
        user.setSingleAttribute("custom.photo", Objects.toString(vinculo.getFoto(), ""));
        user.setSingleAttribute("custom.unitId", String.valueOf(vinculo.getUnidadeId()));
        user.setSingleAttribute("custom.unitName", Objects.toString(vinculo.getNomeUnidade(), ""));
        user.setSingleAttribute("custom.courseId", String.valueOf(vinculo.getCursoId()));
        user.setSingleAttribute("custom.entryYear", String.valueOf(vinculo.getAnoIngresso()));
        user.setSingleAttribute("custom.statusDescription", Objects.toString(vinculo.getDescricaoSituacao(), ""));
        user.setSingleAttribute("custom.userType", String.valueOf(vinculo.getTipo()));
    }

    private void tryPublishOrOutbox(UserModel user, RealmModel realm) {
        if (user.hasRole(realm.getRole("admin"))) return;
        
        String json = UserEventBuilder.buildUserLoggedInEvent(user);
        RabbitMQPublisher publisher = new RabbitMQPublisher();
        boolean published = publisher.tryPublish(json);
        if (!published) {
            DataSource dataSource = DataSourceProvider.get();
            OutboxRepository outbox = new OutboxRepository(dataSource);
            outbox.save("USER_LOGGED_IN", json);
        }
    }
}
