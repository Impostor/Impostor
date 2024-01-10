# HTTP Server

Since Impostor 1.9.0 a HTTP service is included for matchmaking. Recent versions of Among Us require this service in order to connect correctly and not kick players after a round.

Depending on whether you want to support mobile players, you can set up the HTTP server in one of two ways:

- Directly expose the HTTP server. Simpler, but only works if you don't want to support mobile players.
- Use a reverse proxy to expose the HTTP server. More complex, but allows mobile players to connect.

## Directly expose the HTTP server.

In config.json, set "ListenIp" to "0.0.0.0" in the "HttpServer" section. If you don't have this section, look in the example [config.json](https://github.com/Impostor/Impostor/blob/master/src/Impostor.Server/config.json)

## Use a reverse proxy

A reverse proxy allows you to forward HTTP requests from users to multiple services. If you already have one, you should configure it to add Impostor. This page contains an Nginx configuration you can use for reference.

If you have never set up a reverse proxy before, we recommend you to set up [Caddy](https://caddyserver.com/). It is easy to set up and comes with support for requesting SSL certificates out of the box.

To prevent people from connecting directly to Impostor, we recommend changing the "ListenIp" in the "HttpServer" section to "127.0.0.1". This makes sure people can't connect to your HTTP server other than via your reverse proxy. Keep the "ListenIp" in the "Server" section at "0.0.0.0" though, running the normal game traffic through a proxy is not supported.

### Caddy

To install Caddy, follow the [official installation guide](https://caddyserver.com/docs/install). Then use the following lines as your `Caddyfile` configuration file:

```
example.com # replace example.com with your domain name

reverse_proxy :22023
```

Now run `caddy run` in the folder with this Caddyfile and it should set up a server for you with a free SSL certificate.
If this works, you should set up [Caddy to run in the background](https://caddyserver.com/docs/running)

### Nginx

Nginx is an alternative to Caddy that is a bit harder to set up. If you already use Nginx, you can use our snippet to add Impostor:

<details><summary>Nginx configuration</summary>

```nginx
server {
    listen 443 ssl http2;
    server_name example.com; # replace example.com with your domain name

    # Assuming you're using Certbot, replace example.com with your domain name
    ssl_certificate /etc/letsencrypt/live/example.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/example.com/privkey.pem;
    ssl_trusted_certificate /etc/letsencrypt/live/example.com/fullchain.pem;

    location / {
        proxy_pass http://localhost:22023; # Change the port to your HttpServer's ListenPort
        proxy_set_header X-Forwarded-For $remote_addr;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

# Redirect all traffic to HTTPS
server {
    listen 80 default_server;
    location / {
        return 307 https://$host$request_uri;
    }
}
```

</details>
