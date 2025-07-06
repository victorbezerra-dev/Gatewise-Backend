package com.gatewise.keycloak.university;

import java.nio.charset.StandardCharsets;

import com.rabbitmq.client.Channel;
import com.rabbitmq.client.Connection;
import com.rabbitmq.client.ConnectionFactory;

public class RabbitMQPublisher {

    private static final String QUEUE_NAME = "user.login.sync";

    public boolean tryPublish(String message) {
        ConnectionFactory factory = new ConnectionFactory();
        factory.setHost("rabbitmq");
        factory.setPort(5672);
        factory.setUsername("guest");
        factory.setPassword("guest");
        factory.setAutomaticRecoveryEnabled(true);

        try (Connection connection = factory.newConnection(); Channel channel = connection.createChannel()) {

            channel.confirmSelect();

            channel.basicPublish("", QUEUE_NAME, null, message.getBytes(StandardCharsets.UTF_8));
            channel.waitForConfirmsOrDie(5000);

            return true;

        } catch (Exception e) {
            System.err.println("Error publishing to RabbitMQ (with confirmation):" + e.getMessage());
            return false;
        }
    }

}
