package com.gatewise.keycloak.university;

import java.io.IOException;

import org.keycloak.authentication.AuthenticationFlowContext;
import org.keycloak.authentication.AuthenticationFlowError;
import org.keycloak.authentication.Authenticator;
import org.keycloak.authentication.authenticators.browser.UsernamePasswordForm;
import org.keycloak.models.RealmModel;
import org.keycloak.models.UserModel;

import com.gatewise.keycloak.university.dto.AuthResponseDTO;

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

        RealmModel realm = context.getRealm();
        UserModel user = context.getSession().users().getUserByUsername(realm, username);
        if (user == null) {
            user = context.getSession().users().addUser(realm, username);
            user.setEnabled(true);

            String firstName = "";
            String lastName = "";

            String fullName = null;
            if (response.getNome() != null) {
                fullName = response.getNome().trim();
            } else if (response.getVinculo() != null && response.getVinculo().getNome() != null) {
                fullName = response.getVinculo().getNome().trim();
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
            if (response.getVinculo() != null && response.getVinculo().getEmail() != null) {
                email = response.getVinculo().getEmail();
            }
            user.setEmail(email);
            user.setUsername(username);
        }

        context.setUser(user);
        context.success();
    }
}
