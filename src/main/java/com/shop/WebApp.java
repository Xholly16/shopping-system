package com.shop;

import io.javalin.Javalin;
import java.util.Map;

public class WebApp {
    public record AddRequest(int productId, int quantity) {}

    public static void main(String[] args) {
        Catalog catalog = new Catalog();
        Cart cart = new Cart();
        CheckoutService checkout = new CheckoutService();

        Javalin app = Javalin.create(cfg -> cfg.staticFiles.add("/public"));

        app.get("/api/products", ctx -> ctx.json(catalog.getAll()));

        app.get("/api/cart", ctx -> ctx.json(cartView(cart)));

        app.post("/api/cart", ctx -> {
            AddRequest req = ctx.bodyAsClass(AddRequest.class);
            Product p = catalog.findById(req.productId())
                    .orElseThrow(() -> new IllegalArgumentException("Product not found"));
            cart.add(p, req.quantity());
            ctx.json(cartView(cart));
        });

        app.delete("/api/cart/{id}", ctx -> {
            cart.remove(ctx.pathParamAsClass("id", Integer.class).get());
            ctx.json(cartView(cart));
        });

        app.post("/api/checkout", ctx -> ctx.json(checkout.checkout(cart)));

        app.exception(IllegalArgumentException.class,
                (e, ctx) -> ctx.status(400).json(Map.of("error", e.getMessage())));
        app.exception(IllegalStateException.class,
                (e, ctx) -> ctx.status(400).json(Map.of("error", e.getMessage())));

        app.start(7070);
    }

    private static Map<String, Object> cartView(Cart cart) {
        return Map.of("items", cart.getItems(), "total", cart.total());
    }
}
