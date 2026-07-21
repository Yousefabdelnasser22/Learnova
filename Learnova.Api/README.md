# Learnova API Configuration

## Stripe

Stripe secrets must not be stored in appsettings files.

For local development, store secrets with user secrets:

```powershell
cd Learnova.Api
dotnet user-secrets init
dotnet user-secrets set "Stripe:SecretKey" "sk_test_xxxxx"
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_xxxxx"
```

For production, provide secrets through environment variables:

```text
Stripe__SecretKey=sk_live_xxxxx
Stripe__WebhookSecret=whsec_xxxxx
```

Non-secret Stripe settings such as `Stripe:SuccessUrl` and `Stripe:CancelUrl` can remain in appsettings files.
