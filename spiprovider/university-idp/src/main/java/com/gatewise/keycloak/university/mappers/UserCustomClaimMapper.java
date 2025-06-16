package com.gatewise.keycloak.university.mappers;

import java.util.ArrayList;
import java.util.List;

import org.keycloak.models.ClientSessionContext;
import org.keycloak.models.KeycloakSession;
import org.keycloak.models.ProtocolMapperModel;
import org.keycloak.models.UserModel;
import org.keycloak.models.UserSessionModel;
import org.keycloak.protocol.oidc.mappers.AbstractOIDCProtocolMapper;
import org.keycloak.protocol.oidc.mappers.OIDCAccessTokenMapper;
import org.keycloak.protocol.oidc.mappers.OIDCAttributeMapperHelper;
import org.keycloak.protocol.oidc.mappers.OIDCIDTokenMapper;
import org.keycloak.protocol.oidc.mappers.UserInfoTokenMapper;
import org.keycloak.provider.ProviderConfigProperty;
import org.keycloak.representations.IDToken;

public class UserCustomClaimMapper extends AbstractOIDCProtocolMapper
        implements OIDCAccessTokenMapper, OIDCIDTokenMapper, UserInfoTokenMapper {

    public static final String PROVIDER_ID = "user-custom-claim-mapper";

    private static final List<ProviderConfigProperty> configProperties = new ArrayList<>();

    private static final List<String> CLAIM_KEYS = List.of(
            "name", "description", "registration", "email", "photo",
            "unitId", "unitName", "courseId", "entryYear", "statusDescription"
    );

    static {
        OIDCAttributeMapperHelper.addTokenClaimNameConfig(configProperties);
        OIDCAttributeMapperHelper.addIncludeInTokensConfig(configProperties, UserCustomClaimMapper.class);
    }

    @Override
    public String getDisplayCategory() {
        return "Token Mapper";
    }

    @Override
    public String getDisplayType() {
        return "User Custom Claims";
    }

    @Override
    public String getHelpText() {
        return "Adiciona claims personalizadas do usuário baseadas nos atributos do usuário (UserModel).";
    }

    @Override
    public List<ProviderConfigProperty> getConfigProperties() {
        return configProperties;
    }

    @Override
    protected void setClaim(IDToken token, ProtocolMapperModel mappingModel, UserSessionModel userSession,
            KeycloakSession keycloakSession, ClientSessionContext clientSessionCtx) {
        UserModel user = userSession.getUser();
        for (String key : CLAIM_KEYS) {
            String value = user.getFirstAttribute("custom." + key);
            if (value != null) {
                Object typedValue = tryParse(value);
                token.getOtherClaims().put(key, typedValue);
            }
        }
    }

    private Object tryParse(String value) {
        try {
            return Integer.valueOf(value);
        } catch (NumberFormatException e) {
            return value;
        }
    }

    @Override
    public String getId() {
        return PROVIDER_ID;
    }
}
