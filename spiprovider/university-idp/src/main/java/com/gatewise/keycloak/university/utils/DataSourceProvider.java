package com.gatewise.keycloak.university.utils;

import javax.sql.DataSource;

import com.zaxxer.hikari.HikariConfig;
import com.zaxxer.hikari.HikariDataSource;

public class DataSourceProvider {

    private static final HikariDataSource dataSource;

    static {
        HikariConfig config = new HikariConfig();
        config.setJdbcUrl("jdbc:postgresql://gatewise-db:5432/gatewise");
        config.setUsername("gatewise");
        config.setPassword("gatewise");
        config.setMaximumPoolSize(10); 
        config.setConnectionTimeout(3000); 
        config.setIdleTimeout(60000); 
        config.setMaxLifetime(1800000); 

        dataSource = new HikariDataSource(config);
    }

    public static DataSource get() {
        return dataSource;
    }
}
