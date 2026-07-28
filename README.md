# Learnova API

Learnova is a learning-management REST API built with ASP.NET Core 8. It covers
course authoring and publishing, lessons and modules, quizzes, enrollments,
progress tracking, certificates, reviews, favorites, carts, orders, and Stripe
payments.

## Live demo

- [Swagger UI](https://learnova-api.runasp.net/swagger)
- [Health check](https://learnova-api.runasp.net/health)

The demo is deployed on the MonsterASP.NET free tier, so the first request can
take a few seconds while the application wakes up. Stripe payments run in
sandbox mode and don't process real money.

The solution follows a layered architecture:

```text
Learnova.Api             HTTP endpoints, authentication, middleware, Swagger
Learnova.Application     Use cases, validation, DTOs, and application services
Learnova.Domain          Entities, enums, specifications, and abstractions
Learnova.Infrastructure  EF Core, SQL Server, email, Stripe, Qdrant, and jobs
```

## Main technologies

- .NET 8 / ASP.NET Core Web API
- ASP.NET Core Identity with bearer-token authentication
- Entity Framework Core 8 and SQL Server
- Redis output caching
- Hangfire background jobs with SQL Server storage
- Stripe Checkout and webhooks
- Qdrant vector search with Gemini embeddings
- MailKit SMTP email delivery
- MediatR, FluentValidation, AutoMapper, Serilog, and Swagger

## Prerequisites

Install or provision the following before running the API:

- [.NET SDK 8.0.419](global.json), or a compatible .NET 8 SDK
- SQL Server, SQL Server Express, or SQL Server LocalDB
- Redis, reachable through a StackExchange.Redis connection string
- A TLS-enabled Qdrant instance and API key (for example, Qdrant Cloud)
- A Gemini API key
- SMTP credentials
- Stripe test-mode API credentials

Docker is optional. For example, a local Redis instance can be started with:

```powershell
docker run --name learnova-redis -p 6379:6379 -d redis:7-alpine
```

## Quick start

### 1. Restore the solution

From the repository root:

```powershell
dotnet restore Learnova.sln
```

### 2. Configure local secrets

The API project already has a `UserSecretsId`. Keep passwords and API keys out
of `appsettings*.json` and configure them with .NET user secrets during local
development.

The following example uses SQL Server LocalDB. Replace every placeholder with
your own development value:

```powershell
$project = "Learnova.Api/Learnova.Api.csproj"
$database = "Server=(localdb)\MSSQLLocalDB;Database=Learnova;Trusted_Connection=True;TrustServerCertificate=True"

dotnet user-secrets set "ConnectionStrings:cs" $database --project $project
dotnet user-secrets set "ConnectionStrings:HangfireConnection" $database --project $project
dotnet user-secrets set "ConnectionStrings:Redis" "localhost:6379" --project $project

dotnet user-secrets set "Email:Host" "smtp.example.com" --project $project
dotnet user-secrets set "Email:Port" "587" --project $project
dotnet user-secrets set "Email:Username" "developer@example.com" --project $project
dotnet user-secrets set "Email:Password" "<smtp-app-password>" --project $project
dotnet user-secrets set "Email:FromEmail" "no-reply@example.com" --project $project
dotnet user-secrets set "Email:FromName" "Learnova" --project $project

dotnet user-secrets set "Qdrant:Host" "<qdrant-host-without-https>" --project $project
dotnet user-secrets set "Qdrant:Port" "6334" --project $project
dotnet user-secrets set "Qdrant:ApiKey" "<qdrant-api-key>" --project $project

dotnet user-secrets set "Gemini:ApiKey" "<gemini-api-key>" --project $project
dotnet user-secrets set "Gemini:EmbeddingModel" "gemini-embedding-001" --project $project

dotnet user-secrets set "Stripe:SecretKey" "sk_test_xxxxx" --project $project
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_xxxxx" --project $project

dotnet user-secrets set "AdminBootstrap:Email" "admin@example.com" --project $project
dotnet user-secrets set "AdminBootstrap:Password" "<strong-one-time-password>" --project $project
```

Notes:

- Use the same SQL Server database for the application and Hangfire unless you
  intentionally want separate databases.
- The current Qdrant client configuration uses HTTPS. Supply a hostname only,
  without `https://`, and use the provider's gRPC/TLS port (normally `6334`).
- All Email, Qdrant, Gemini, and Stripe settings are validated at startup. The
  API will not start if one of the required values is empty.
- User secrets are loaded only in the Development environment. Use environment
  variables or a managed secret store in production.

To review or remove local secrets:

```powershell
dotnet user-secrets list --project Learnova.Api/Learnova.Api.csproj
dotnet user-secrets clear --project Learnova.Api/Learnova.Api.csproj
```

### 3. Apply database migrations

Install the .NET 8 EF Core command-line tool if it is not already installed:

```powershell
dotnet tool install --global dotnet-ef --version 8.*
```

Apply the existing migrations:

```powershell
dotnet ef database update `
  --project Learnova.Infrastructure/Learnova.Infrastructure.csproj `
  --startup-project Learnova.Api/Learnova.Api.csproj
```

When the API runs in Development, it also applies pending migrations
automatically. In every environment, application startup seeds the `Admin`,
`Student`, and `Instructor` roles. If no Admin exists, it creates the initial
Admin from `AdminBootstrap:Email` and `AdminBootstrap:Password`. Remove the
bootstrap password from configuration after the account has been created.
Users created through `/api/identity/register` receive the `Student` role
automatically.

For production deployments, run `dotnet ef database update` as an explicit
deployment step; the application does not apply migrations automatically in
Production.

To create a new migration after changing the EF Core model:

```powershell
dotnet ef migrations add <MigrationName> `
  --project Learnova.Infrastructure/Learnova.Infrastructure.csproj `
  --startup-project Learnova.Api/Learnova.Api.csproj `
  --output-dir Migrations
```

### 4. Build and run

```powershell
dotnet build Learnova.sln
dotnet run --project Learnova.Api/Learnova.Api.csproj --launch-profile https
```

Development URLs from `launchSettings.json`:

- HTTPS API: `https://localhost:7280`
- HTTP API: `http://localhost:5208`
- Swagger UI: `https://localhost:7280/swagger`
- Hangfire dashboard: `https://localhost:7280/hangfire`

The Hangfire dashboard requires an authenticated user with the `Admin` role.

If the local HTTPS certificate is not trusted, run:

```powershell
dotnet dev-certs https --trust
```

## Configuration reference

| Setting | Required | Sensitive | Purpose |
| --- | --- | --- | --- |
| `ConnectionStrings:cs` | Yes | Usually | Main SQL Server database |
| `ConnectionStrings:HangfireConnection` | Yes | Usually | Hangfire SQL Server storage |
| `ConnectionStrings:Redis` | Yes | Sometimes | Distributed output cache |
| `Email:Host` | Yes | No | SMTP server hostname |
| `Email:Port` | Yes | No | SMTP server port |
| `Email:Username` | Yes | Yes | SMTP account username |
| `Email:Password` | Yes | Yes | SMTP password or app password |
| `Email:FromEmail` | Yes | No | Sender email address |
| `Email:FromName` | Yes | No | Sender display name |
| `Qdrant:Host` | Yes | No | Qdrant hostname without a URL scheme |
| `Qdrant:Port` | Yes | No | Qdrant gRPC/TLS port; defaults to `6334` |
| `Qdrant:ApiKey` | Yes | Yes | Qdrant API key |
| `Gemini:ApiKey` | Yes | Yes | Gemini embeddings API key |
| `Gemini:EmbeddingModel` | Yes | No | Embedding model name |
| `Stripe:SecretKey` | Yes | Yes | Stripe server-side secret key |
| `Stripe:WebhookSecret` | Yes | Yes | Stripe webhook signing secret |
| `Stripe:SuccessUrl` | Yes | No | Checkout success redirect URL |
| `Stripe:CancelUrl` | Yes | No | Checkout cancellation redirect URL |
| `Cors:AllowedOrigins` | Yes | No | Frontend origins allowed by CORS |
| `AdminBootstrap:Email` | Until the first Admin exists | No | Email for the initial Admin account |
| `AdminBootstrap:Password` | Until the first Admin exists | Yes | One-time password for the initial Admin account |

ASP.NET Core maps double underscores in environment-variable names to nested
configuration keys. For example:

```text
ConnectionStrings__cs=<sql-connection-string>
ConnectionStrings__HangfireConnection=<sql-connection-string>
ConnectionStrings__Redis=<redis-connection-string>
Email__Host=<smtp-host>
Email__Port=587
Email__Username=<smtp-username>
Email__Password=<smtp-password>
Email__FromEmail=<sender-address>
Email__FromName=Learnova
Qdrant__Host=<qdrant-host>
Qdrant__Port=6334
Qdrant__ApiKey=<qdrant-api-key>
Gemini__ApiKey=<gemini-api-key>
Gemini__EmbeddingModel=gemini-embedding-001
Stripe__SecretKey=<stripe-secret-key>
Stripe__WebhookSecret=<stripe-webhook-secret>
Stripe__SuccessUrl=<frontend-success-url>
Stripe__CancelUrl=<frontend-cancel-url>
Cors__AllowedOrigins__0=<frontend-origin>
AdminBootstrap__Email=<initial-admin-email>
AdminBootstrap__Password=<strong-one-time-password>
```

## Stripe webhooks

For local Stripe webhook testing, forward Stripe events to the anonymous
webhook endpoint:

```powershell
stripe listen --forward-to https://localhost:7280/api/payments/stripe/webhook
```

Copy the `whsec_...` value printed by the Stripe CLI into
`Stripe:WebhookSecret`. The endpoint verifies Stripe signatures before
processing checkout completion, checkout expiration, and refund events.

## Development startup behavior

In the Development environment, application startup performs these steps:

1. Applies pending EF Core migrations.
2. Seeds the `Admin`, `Student`, and `Instructor` roles and creates the first
   Admin when necessary.
3. Ensures that the Qdrant `courses` collection exists with 3,072-dimensional
   cosine-distance vectors.

If Qdrant is configured but unreachable, the error is logged and the API keeps
running, but semantic course search remains unavailable. SQL Server must be
reachable for migrations and seeding to succeed.

## Production checklist

- Set `ASPNETCORE_ENVIRONMENT=Production`.
- Supply all sensitive values through environment variables or a managed secret
  store; never commit them to Git.
- Replace localhost frontend URLs and CORS origins with the deployed frontend
  URL.
- Use production SQL Server, Redis, SMTP, Qdrant, Gemini, and Stripe resources.
- Apply EF Core migrations before starting the new application version.
- Provide `AdminBootstrap__Email` and `AdminBootstrap__Password` for the first
  startup, then remove the password after the Admin account is created.
- Configure HTTPS and a reverse proxy or managed application host.
- Configure the Stripe production webhook to send events to
  `/api/payments/stripe/webhook`.
- Restrict access to operational endpoints such as the Hangfire dashboard.

## Troubleshooting

### The application fails with an options-validation error

Run `dotnet user-secrets list` and verify that every Email, Qdrant, Gemini, and
Stripe setting in the configuration table has a non-empty value.

### SQL Server login or connection fails

Verify the server name, database authentication mode, and TLS options in both
SQL connection strings. With LocalDB, ensure that the `MSSQLLocalDB` instance is
installed and running.

### Redis is unavailable

Verify that Redis is listening on the host and port in
`ConnectionStrings:Redis`. The default development value is `localhost:6379`.

### Qdrant initialization fails

Use the Qdrant hostname without `https://`, confirm the gRPC/TLS port, and check
that the API key can list and create collections.

### A registered user cannot sign in

Identity requires email confirmation. Confirm that the SMTP configuration can
deliver the verification message and that the user completed the confirmation
flow.
