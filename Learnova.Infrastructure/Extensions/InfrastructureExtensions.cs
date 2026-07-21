using Learnova.Application.Common.BackgroundJobs;
using Learnova.Application.Courses.Services;
using Learnova.Application.Payments.Gateway;
using Learnova.Domain.Interfaces;
using Learnova.Infrastructure.BackgroundJobs;
using Learnova.Infrastructure.Configuration;
using Learnova.Infrastructure.Data;
using Learnova.Infrastructure.Email;
using Learnova.Infrastructure.Payments.Stripe;
using Learnova.Infrastructure.Repositories;
using Learnova.Infrastructure.Search;
using Learnova.Infrastructure.Seeder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Qdrant.Client;

namespace Learnova.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static void AddInfrastructureService(
            this IServiceCollection services,
            IConfigurationManager configuration,
            string environmentName)
        {
            services.AddDbContext<AppDbContext>(op =>
            {
                op.UseSqlServer(configuration.GetConnectionString("cs"));

                if (string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase))
                {
                    op.EnableSensitiveDataLogging();
                }
            });

            services.AddScoped<Iseeder, Learnova.Infrastructure.Seeder.Seeder>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPaymentGatewayService, StripePaymentGatewayService>();
            services.AddScoped<IBackgroundJobScheduler, HangfireBackgroundJobScheduler>();

            services
                .AddOptions<EmailSettings>()
                .Bind(configuration.GetSection(EmailSettings.SectionName))
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.Host),
                    "Email Host is required.")
                .Validate(settings => settings.Port is > 0 and <= 65535,
                    "Email Port must be between 1 and 65535.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.Username),
                    "Email Username is required.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.Password),
                    "Email Password is required.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.FromEmail),
                    "Email FromEmail is required.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.FromName),
                    "Email FromName is required.")
                .ValidateOnStart();

            services.AddTransient<IEmailSender<Learnova.Domain.Entites.ApplicationUser>, SmtpEmailSender>();

            services
                .AddOptions<QdrantSettings>()
                .Bind(configuration.GetSection(QdrantSettings.SectionName))
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.Host),
                    "Qdrant Host is required.")
                .Validate(settings => settings.Port is > 0 and <= 65535,
                    "Qdrant Port must be between 1 and 65535.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.ApiKey),
                    "Qdrant ApiKey is required.")
                .ValidateOnStart();

            services
                .AddOptions<GeminiSettings>()
                .Bind(configuration.GetSection(GeminiSettings.SectionName))
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.ApiKey),
                    "Gemini ApiKey is required.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.EmbeddingModel),
                    "Gemini EmbeddingModel is required.")
                .ValidateOnStart();

            services.AddSingleton<QdrantClient>(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<QdrantSettings>>().Value;

                return new QdrantClient(
                    host: settings.Host,
                    port: settings.Port,
                    https: true,
                    apiKey: settings.ApiKey);
            });

            services.AddScoped(provider =>
                new EmbeddingService(
                    provider.GetRequiredService<IOptions<GeminiSettings>>(),
                    new HttpClient
                    {
                        Timeout = TimeSpan.FromSeconds(15)
                    }));
            services.AddScoped<ICourseSearchService, CourseSearchService>();
            services.AddScoped<QdrantInitializer>();

            services
                .AddOptions<StripeSettings>()
                .Bind(configuration.GetSection(StripeSettings.SectionName))
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.SecretKey),
                    "Stripe SecretKey is required.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.WebhookSecret),
                    "Stripe WebhookSecret is required.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.SuccessUrl),
                    "Stripe SuccessUrl is required.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.CancelUrl),
                    "Stripe CancelUrl is required.")
                .ValidateOnStart();
        }
    }
}
