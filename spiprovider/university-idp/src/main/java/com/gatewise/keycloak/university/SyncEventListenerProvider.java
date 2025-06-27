package com.gatewise.keycloak.university;

import java.io.IOException;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;

import org.keycloak.events.Event;
import org.keycloak.events.EventListenerProvider;
import org.keycloak.events.EventType;
import org.keycloak.events.admin.AdminEvent;

public class SyncEventListenerProvider implements EventListenerProvider {

    private final HttpClient client = HttpClient.newHttpClient();

    @Override
    public void onEvent(Event event) {
        if (event.getType() == EventType.LOGIN) {
            String userId = event.getUserId();
            System.out.println("Logged in user ID" + userId);

            try {
                //TODO: Send to queue (Ex: RabbitMQ)
                HttpRequest request = HttpRequest.newBuilder()
                        .uri(URI.create("http://example-api.local/api/Users/sync/" + userId))
                        .POST(HttpRequest.BodyPublishers.noBody())
                        .build();

                client.send(request, HttpResponse.BodyHandlers.ofString());
            } catch (IOException | InterruptedException e) {
                System.err.println("Error synchronizing user: " + e.getMessage());
            }
        }
    }

    @Override
    public void close() {
    }

    @Override
    public void onEvent(AdminEvent event, boolean includeRepresentation) {
        throw new UnsupportedOperationException("Not supported yet.");
    }
}
