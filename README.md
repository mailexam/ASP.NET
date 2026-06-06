# ASP.NET Core + Mailexam

Minimal [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) example that sends test mail through [Mailexam](https://mailexam.io/) SMTP via [MailKit](https://github.com/jstedfast/MailKit).

Based on the [Mailexam ASP.NET Core guide](https://wiki.mailexam.ru/en/examples/aspnet/).

## What you need

- A Mailexam account and a project with SMTP credentials.
- [.NET SDK](https://dotnet.microsoft.com/download) 8 or newer.

From your Mailexam welcome email or dashboard:

| Variable | Description |
|----------|-------------|
| `MAILEXAM_LOGIN` | SMTP login (for example, `xxxxx`) |
| `MAILEXAM_PASSWORD` | SMTP password (paired with the login) |
| Host | `{MAILEXAM_LOGIN}.mailexam.io` (built in `Services/MailService.cs`) |

## Quick start (host)

1. Restore dependencies:

```bash
dotnet restore
```

2. Export Mailexam credentials (or copy from the example file):

```bash
cp .env.example .env
export $(grep -v '^#' .env | xargs)
```

3. Edit `.env`:

```env
MAILEXAM_LOGIN=YOUR_LOGIN
MAILEXAM_PASSWORD=YOUR_PASSWORD
MAILEXAM_PORT=587
MAIL_FROM=noreply@example.test
```

4. Run the application:

```bash
dotnet run
```

The server listens on `http://127.0.0.1:8080` by default.

5. Send a test message:

```bash
curl -X POST http://127.0.0.1:8080/mail/test \
  -H 'Content-Type: application/json' \
  -d '{"to":"user@example.test","subject":"Test","body":"Hello"}'
```

The message appears in the Mailexam dashboard → your project → inbox.

### User Secrets (optional, local)

```bash
dotnet user-secrets init
dotnet user-secrets set MAILEXAM_LOGIN YOUR_LOGIN
dotnet user-secrets set MAILEXAM_PASSWORD YOUR_PASSWORD
```

The example reads credentials from environment variables; export them or use CI secrets in production.

## Environment variables

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `MAILEXAM_LOGIN` | yes | — | SMTP login; also used to build the host name |
| `MAILEXAM_PASSWORD` | yes | — | SMTP password |
| `MAILEXAM_PORT` | no | `587` | SMTP port (`587`, `2525`, `465`, or `25`) |
| `MAIL_FROM` | no | `noreply@example.test` | Sender address (any test address is fine) |
| `HTTP_HOST` | no | `127.0.0.1` | HTTP bind address |
| `HTTP_PORT` | no | `8080` | HTTP listen port |

For port **587** and **2525**, MailKit uses `SecureSocketOptions.StartTls`. For port **465** it uses `SslOnConnect`. For port **25** it uses `None`.

## Project layout

```
.
├── ASP.NET.csproj
├── Program.cs                    # Minimal API POST /mail/test
├── Services/MailService.cs
├── .env.example
├── Dockerfile                    # for local debugging only
└── docker-compose.yml
```

## Docker (debugging)

Docker is provided for local debugging. For day-to-day development, run the app on the host with `dotnet run` (see above).

```bash
cp .env.example .env
# edit .env with your credentials

docker compose up --build
```

Then call the same endpoint on the mapped port:

```bash
curl -X POST http://127.0.0.1:8080/mail/test \
  -H 'Content-Type: application/json' \
  -d '{"to":"user@example.test","subject":"Test","body":"Hello"}'
```

Inside the container the server binds to `0.0.0.0:8080`.

## CI

Set these secrets in your CI environment:

```yaml
variables:
  MAILEXAM_LOGIN: $MAILEXAM_LOGIN
  MAILEXAM_PASSWORD: $MAILEXAM_PASSWORD
  MAILEXAM_PORT: "587"
  MAIL_FROM: "noreply@example.test"
```

After sending a message in a test, verify delivery via the [Mailexam API](https://mailexam.io/api).

## Troubleshooting

**TLS or authentication failed**

- Host must be `{login}.mailexam.io`, where `{login}` matches `MAILEXAM_LOGIN`.
- Login and password must come from the same Mailexam project.

**Port 587**

- Use `SecureSocketOptions.StartTls`, not `SslOnConnect`.

**Message not in the dashboard**

- Open the inbox of the same Mailexam project.
- Enable verbose logs in `appsettings.Development.json` if needed.

## See also

- [Mailexam ASP.NET Core guide (wiki)](https://wiki.mailexam.ru/en/examples/aspnet/)
- [Spring Boot reference implementation](https://github.com/mailexam/Spring) — SMTP on JVM
- [MailKit documentation](https://mimekit.net/docs/html/Introduction.htm)
- [Mailexam API documentation](https://mailexam.io/api)
