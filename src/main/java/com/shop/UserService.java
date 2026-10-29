package com.shop;

import java.util.Map;
import java.util.Optional;
import java.util.concurrent.ConcurrentHashMap;

public class UserService {
    private final Map<String, User> users = new ConcurrentHashMap<>();

    public synchronized User register(String username, String email, String password) {
        if (username == null || !username.matches("[A-Za-z0-9_]{3,20}")) {
            throw new IllegalArgumentException("Username must be 3-20 letters, numbers or underscores");
        }
        if (email == null || !email.matches("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$")) {
            throw new IllegalArgumentException("Enter a valid email address");
        }
        if (password == null || password.length() < 8) {
            throw new IllegalArgumentException("Password must be at least 8 characters");
        }
        String key = username.toLowerCase();
        if (users.containsKey(key)) {
            throw new IllegalArgumentException("Username already taken");
        }
        User user = new User(username, email, PasswordHasher.hash(password));
        users.put(key, user);
        return user;
    }

    public User login(String username, String password) {
        User user = username == null ? null : users.get(username.toLowerCase());
        if (user == null || password == null
                || !PasswordHasher.verify(password, user.getPasswordHash())) {
            throw new IllegalArgumentException("Invalid username or password");
        }
        return user;
    }

    public Optional<User> find(String username) {
        return Optional.ofNullable(users.get(username.toLowerCase()));
    }
}
