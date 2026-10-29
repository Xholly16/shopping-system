package com.shop;

import static org.junit.jupiter.api.Assertions.*;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.ValueSource;

class UserServiceTest {
    private UserService users;

    @BeforeEach
    void setUp() { users = new UserService(); }

    @Test
    void register_thenLogin_works() {
        users.register("nhlanhla", "n@test.com", "password123");
        assertEquals("nhlanhla", users.login("nhlanhla", "password123").getUsername());
    }

    @Test
    void password_isNotStoredInPlainText() {
        User u = users.register("nhlanhla", "n@test.com", "password123");
        assertNotEquals("password123", u.getPasswordHash());
        assertFalse(u.getPasswordHash().contains("password123"));
    }

    @Test
    void duplicateUsername_isRejected_ignoringCase() {
        users.register("Nhlanhla", "n@test.com", "password123");
        assertThrows(IllegalArgumentException.class,
                () -> users.register("nhlanhla", "x@test.com", "password123"));
    }

    @Test
    void wrongPassword_isRejected() {
        users.register("nhlanhla", "n@test.com", "password123");
        assertThrows(IllegalArgumentException.class, () -> users.login("nhlanhla", "wrongpass1"));
    }

    @Test
    void unknownUser_isRejected() {
        assertThrows(IllegalArgumentException.class, () -> users.login("ghost", "password123"));
    }

    @Test
    void shortPassword_isRejected() {
        assertThrows(IllegalArgumentException.class,
                () -> users.register("nhlanhla", "n@test.com", "short"));
    }

    @ParameterizedTest
    @ValueSource(strings = {"", "ab", "has space", "bad!char", "waytoolongusername12345"})
    void invalidUsernames_areRejected(String name) {
        assertThrows(IllegalArgumentException.class,
                () -> users.register(name, "n@test.com", "password123"));
    }

    @ParameterizedTest
    @ValueSource(strings = {"", "plain", "no@dot", "@test.com", "a b@test.com"})
    void invalidEmails_areRejected(String email) {
        assertThrows(IllegalArgumentException.class,
                () -> users.register("nhlanhla", email, "password123"));
    }
}
