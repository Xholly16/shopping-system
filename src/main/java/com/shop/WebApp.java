package com.shop;

import io.javalin.Javalin;
import io.javalin.http.Context;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;
import java.util.function.Function;

public class WebApp {
    public record SignupRequest(String username, String email, String password) {}
    public record LoginRequest(String username, String password) {}
    public record AddRequest(int productId, int quantity) {}
    public record CheckoutRequest(String method, String cardNumber, String expiry, String cvv) {}

    static class AuthException extends RuntimeException {
        AuthException(String message) { super(message); }
    }

    private static final Set<String> PUBLIC_PATHS =
            Set.of("/api/products", "/api/signup", "/api/login");

    public static void main(String[] args) {
        createApp().start(7070);
    }

    public static Javalin createApp() {
        Catalog catalog = new Catalog();
        UserService users = new UserService();
        CheckoutService checkout = new CheckoutService();
        Map<String, Cart> carts = new ConcurrentHashMap<>();
        Function<Context, Cart> cartOf =
                ctx -> carts.computeIfAbsent(username(ctx), k -> new Cart());

        Javalin app = Javalin.create(cfg -> cfg.staticFiles.add("/public"));

        app.before("/api/*", ctx -> {
            if (!PUBLIC_PATHS.contains(ctx.path()) && ctx.sessionAttribute("user") == null) {
                throw new AuthException("Please log in first");
            }
        });

        app.post("/api/signup", ctx -> {
            SignupRequest r = ctx.bodyAsClass(SignupRequest.class);
            User user = users.register(r.username(), r.email(), r.password());
            startSession(ctx, user.getUsername());
            ctx.status(201).json(userView(user));
        });

        app.post("/api/login", ctx -> {
            LoginRequest r = ctx.bodyAsClass(LoginRequest.class);
            User user = users.login(r.username(), r.password());
            startSession(ctx, user.getUsername());
            ctx.json(userView(user));
        });

        app.post("/api/logout", ctx -> {
            if (ctx.req().getSession(false) != null) {
                ctx.req().getSession(false).invalidate();
            }
            ctx.json(Map.of("ok", true));
        });

        app.get("/api/me", ctx -> {
            User user = users.find(username(ctx))
                    .orElseThrow(() -> new AuthException("Please log in first"));
            ctx.json(userView(user));
        });

        app.get("/api/products", ctx -> ctx.json(catalog.getAll()));

        app.get("/api/cart", ctx -> ctx.json(cartView(cartOf.apply(ctx))));

        app.post("/api/cart", ctx -> {
            AddRequest r = ctx.bodyAsClass(AddRequest.class);
            Product p = catalog.findById(r.productId())
                    .orElseThrow(() -> new IllegalArgumentException("Product not found"));
            Cart cart = cartOf.apply(ctx);
            cart.add(p, r.quantity());
            ctx.json(cartView(cart));
        });

        app.delete("/api/cart/{id}", ctx -> {
            Cart cart = cartOf.apply(ctx);
            cart.remove(ctx.pathParamAsClass("id", Integer.class).get());
            ctx.json(cartView(cart));
        });

        app.delete("/api/cart", ctx -> {
            Cart cart = cartOf.apply(ctx);
            cart.clear();
            ctx.json(cartView(cart));
        });

        app.post("/api/checkout", ctx -> {
            CheckoutRequest r = ctx.bodyAsClass(CheckoutRequest.class);
            PaymentService.Method method;
            try {
                method = PaymentService.Method.valueOf(String.valueOf(r.method()).toUpperCase());
            } catch (IllegalArgumentException e) {
                throw new IllegalArgumentException("Choose a payment method");
            }
            Order order = checkout.checkout(cartOf.apply(ctx), username(ctx), method,
                    r.cardNumber(), r.expiry(), r.cvv());
            ctx.json(order);
        });

        app.get("/api/orders", ctx -> ctx.json(checkout.ordersFor(username(ctx))));

        app.exception(IllegalArgumentException.class,
                (e, ctx) -> ctx.status(400).json(Map.of("error", e.getMessage())));
        app.exception(IllegalStateException.class,
                (e, ctx) -> ctx.status(400).json(Map.of("error", e.getMessage())));
        app.exception(AuthException.class,
                (e, ctx) -> ctx.status(401).json(Map.of("error", e.getMessage())));

        return app;
    }

    private static void startSession(Context ctx, String username) {
        if (ctx.req().getSession(false) != null) {
            ctx.req().changeSessionId();
        }
        ctx.sessionAttribute("user", username);
    }

    private static String username(Context ctx) {
        String name = ctx.sessionAttribute("user");
        if (name == null) {
            throw new AuthException("Please log in first");
        }
        return name;
    }

    private static Map<String, Object> userView(User user) {
        return Map.of("username", user.getUsername(), "email", user.getEmail());
    }

    private static Map<String, Object> cartView(Cart cart) {
        return Map.of("items", cart.getItems(), "total", cart.total());
    }
}
