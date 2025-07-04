package com.gatewise.keycloak.university.utils;

import java.time.Instant;
import java.util.HashMap;
import java.util.Map;

import org.keycloak.models.UserModel;

import com.google.gson.Gson;

public class UserEventBuilder {

    public static String buildUserLoggedInEvent(UserModel user) {
        Map<String, Object> userMap = new HashMap<>();
        userMap.put("id", user.getId());
        userMap.put("username", user.getUsername());
        userMap.put("name", user.getFirstAttribute("custom.name"));
        userMap.put("description", user.getFirstAttribute("custom.description"));
        userMap.put("registration", user.getFirstAttribute("custom.registration"));
        userMap.put("email", user.getFirstAttribute("custom.email"));
        userMap.put("photo", user.getFirstAttribute("custom.photo"));
        userMap.put("unitId", user.getFirstAttribute("custom.unitId"));
        userMap.put("unitName", user.getFirstAttribute("custom.unitName"));
        userMap.put("courseId", user.getFirstAttribute("custom.courseId"));
        userMap.put("entryYear", user.getFirstAttribute("custom.entryYear"));
        userMap.put("statusDescription", user.getFirstAttribute("custom.statusDescription"));    
        userMap.put("user_type", user.getFirstAttribute("custom.userType")); 

        Map<String, Object> event = new HashMap<>();
        event.put("event", "USER_LOGGED_IN");
        event.put("timestamp", Instant.now().toString());
        event.put("user", userMap);

        return new Gson().toJson(event);
    }
}
