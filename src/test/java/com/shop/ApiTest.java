package com.shop;

import static org.junit.jupiter.api.Assertions.*;

import io.javalin.Javalin;
import java.net.CookieManager;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.UUID;
import org.junit.jupiter.api.AfterAll;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;

class ApiTest {
    private static Javalin app;
    private static String base;

    @BeforeAll
    static void start() {
        app = WebApp.createApp().start(0);
        base = "http://localhost:" + app.port();
    }

    @AfterAll
    static void stop() { app.stop(); }

    /** One browser: keeps its own cookies, so each Session is a separate logged-in user. */
    private static class Session {
        final HttpClient client = HttpClient.newBuilder().cookieHandler(new CookieManager()).build();

        HttpResponse<String> send(String method, String path, String json) throws Exception {
            HttpRequest.Builder b = HttpRequest.newBuilder(URI.create(base + path))
                    .header("Content-Type", "application/json");
            b.method(method, json == null
                    ? HttpRequest.BodyPublishers.noBody()
                    : HttpRequest.BodyPublishers.ofString(json));
            return client.send(b.build(), HttpResponse.BodyHandlers.ofString());
        }

        String signUp() throws Exception {
            String name = "user" + UUID.randomUUID().toString().substring(0, 8);
            HttpResponse<String> r = send("POST", "/api/signup",
                    "{\"username\":\"" + name + "\",\"email\":\"" + name + "@test.com\",\"password\":\"password123\"}");
            assertEquals(201, r.statusCode());
            return name;
        }

        void addUsbCable(int qty) throws Exception {
            assertEquals(200, send("POST", "/api/cart",
                    "{\"productId\":5,\"quantity\":" + qty + "}").statusCode());
        }
    }

    @Test
    void products_arePublic() throws Exception {
        HttpResponse<String> r = new Session().send("GET", "/api/products", null);
        assertEquals(200, r.statusCode());
        assertTrue(r.body().contains("Laptop"));
    }

    @Test
    void cart_requiresLogin() throws Exception {
        assertEquals(401, new Session().send("GET", "/api/cart", null).statusCode());
    }

    @Test
    void orders_requireLogin() throws Exception {
        assertEquals(401, new Session().send("GET", "/api/orders", null).statusCode());
    }

    @Test
    void signup_thenMe_returnsUser() throws Exception {
        Session s = new Session();
        String name = s.signUp();
        HttpResponse<String> me = s.send("GET", "/api/me", null);
        assertEquals(200, me.statusCode());
        assertTrue(me.body().contains(name));
        assertFalse(me.body().contains("password"));
    }

    @Test
    void duplicateSignup_returns400() throws Exception {
        Session s = new Session();
        String name = s.signUp();
        HttpResponse<String> again = new Session().send("POST", "/api/signup",
                "{\"username\":\"" + name + "\",\"email\":\"x@test.com\",\"password\":\"password123\"}");
        assertEquals(400, again.statusCode());
    }

    @Test
    void login_wrongPassword_returns400() throws Exception {
        String name = new Session().signUp();
        HttpResponse<String> r = new Session().send("POST", "/api/login",
                "{\"username\":\"" + name + "\",\"password\":\"wrongpass99\"}");
        assertEquals(400, r.statusCode());
    }

    @Test
    void logout_endsTheSession() throws Exception {
        Session s = new Session();
        s.signUp();
        s.send("POST", "/api/logout", null);
        assertEquals(401, s.send("GET", "/api/cart", null).statusCode());
    }

    @Test
    void fullPurchase_withCard_createsOrder_andNeverLeaksCardNumber() throws Exception {
        Session s = new Session();
        s.signUp();
        s.addUsbCable(2);

        HttpResponse<String> pay = s.send("POST", "/api/checkout",
                "{\"method\":\"CARD\",\"cardNumber\":\"4242424242424242\",\"expiry\":\"12/40\",\"cvv\":\"123\"}");
        assertEquals(200, pay.statusCode());
        assertTrue(pay.body().contains("\"cardLast4\":\"4242\""));
        assertFalse(pay.body().contains("4242424242424242"));

        HttpResponse<String> orders = s.send("GET", "/api/orders", null);
        assertTrue(orders.body().contains("USB Cable"));
        assertFalse(s.send("GET", "/api/cart", null).body().contains("USB Cable"));
    }

    @Test
    void checkout_withBadCard_returns400_andKeepsCart() throws Exception {
        Session s = new Session();
        s.signUp();
        s.addUsbCable(1);

        HttpResponse<String> pay = s.send("POST", "/api/checkout",
                "{\"method\":\"CARD\",\"cardNumber\":\"1234\",\"expiry\":\"12/40\",\"cvv\":\"123\"}");
        assertEquals(400, pay.statusCode());
        assertTrue(s.send("GET", "/api/cart", null).body().contains("USB Cable"));
    }

    @Test
    void checkout_cashOnDelivery_works() throws Exception {
        Session s = new Session();
        s.signUp();
        s.addUsbCable(1);
        HttpResponse<String> pay = s.send("POST", "/api/checkout", "{\"method\":\"CASH_ON_DELIVERY\"}");
        assertEquals(200, pay.statusCode());
        assertTrue(pay.body().contains("CASH_ON_DELIVERY"));
    }

    @Test
    void checkout_emptyCart_returns400() throws Exception {
        Session s = new Session();
        s.signUp();
        HttpResponse<String> pay = s.send("POST", "/api/checkout", "{\"method\":\"CASH_ON_DELIVERY\"}");
        assertEquals(400, pay.statusCode());
        assertTrue(pay.body().contains("Cart is empty"));
    }

    @Test
    void addingMoreThanStock_returns400() throws Exception {
        Session s = new Session();
        s.signUp();
        assertEquals(400, s.send("POST", "/api/cart",
                "{\"productId\":5,\"quantity\":9999}").statusCode());
    }

    @Test
    void carts_areSeparatePerUser() throws Exception {
        Session a = new Session();
        Session b = new Session();
        a.signUp();
        b.signUp();
        a.addUsbCable(1);
        assertTrue(a.send("GET", "/api/cart", null).body().contains("USB Cable"));
        assertFalse(b.send("GET", "/api/cart", null).body().contains("USB Cable"));
    }
}
