package com.gatewise.keycloak.university.repositories;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.UUID;

import javax.sql.DataSource;

public class OutboxRepository {

    private final DataSource dataSource;

    public OutboxRepository(DataSource dataSource) {
        this.dataSource = dataSource;
        ensureTableExists();
    }

    private void ensureTableExists() {
        String sql = "CREATE TABLE IF NOT EXISTS outbox ("
                + "id UUID PRIMARY KEY,"
                + "event_type TEXT NOT NULL,"
                + "payload JSONB NOT NULL,"
                + "status TEXT NOT NULL,"
                + "created_at TIMESTAMP NOT NULL DEFAULT now(),"
                + "updated_at TIMESTAMP,"
                + "attempts INT DEFAULT 0"
                + ")";

        try (Connection conn = dataSource.getConnection(); Statement stmt = conn.createStatement()) {
            stmt.execute(sql);
        } catch (SQLException e) {
            System.err.println("Failed to ensure creation of 'outbox' table: " + e.getMessage());
        }
    }

    public void save(String eventType, String payload) {
        String sql = "INSERT INTO outbox (id, event_type, payload, status) VALUES (?, ?, ?::jsonb, ?)";

        try (Connection conn = dataSource.getConnection(); PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setObject(1, UUID.randomUUID());
            stmt.setString(2, eventType);
            stmt.setString(3, payload);
            stmt.setString(4, "PENDING");

            stmt.executeUpdate();

        } catch (SQLException e) {
            System.err.println("Failed to save event to outbox: " + e.getMessage());
        }
    }
}
