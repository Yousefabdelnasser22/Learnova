using Hangfire;
using Hangfire.SqlServer;
using Learnova.Api.OpenApi;
using Learnova.Api.Services;
using Learnova.Application.Common.BackgroundJobs;
using Learnova.Application.User;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace Learnova.Api.Extensions
{
    public static class WebApplicationExtensions
    {
            public const string FrontendCorsPolicy = "Frontend";
        
            public static void AddPresentation(
                this IServiceCollection services,
                IConfiguration configuration)
            {
                services
                    .AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(
                            new JsonStringEnumConverter(allowIntegerValues: false));
                    });
                services.AddEndpointsApiExplorer();
                services.AddCors(options =>
                {
                    var allowedOrigins = configuration
                        .GetSection("Cors:AllowedOrigins")
                        .Get<string[]>() ?? Array.Empty<string>();

                    options.AddPolicy(FrontendCorsPolicy, policy =>
                    {
                        policy.WithOrigins(allowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
                });

                services.AddScoped<IUserContext, HttpUserContext>();
                services.AddHttpContextAccessor();

                services.AddSwaggerGen(c =>
                {
                    c.EnableAnnotations();
                    c.OperationFilter<TooManyRequestsResponseOperationFilter>();
                    c.OperationFilter<IdentityRegistrationOperationFilter>();

                    c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearerAuth"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                });

                services.AddHangfire(config =>
                {
                    config
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSqlServerStorage(
                            configuration.GetConnectionString("HangfireConnection"),
                            new SqlServerStorageOptions
                            {
                                PrepareSchemaIfNecessary = true
                            });
                });

                services.AddHangfireServer();
            }

            public static void UseRecurringJobs(this WebApplication app)
            {
                RecurringJob.AddOrUpdate<IPendingOrderCleanupJob>(
                    "cleanup-pending-orders",
                    job => job.CleanupAsync(),
                    Cron.Hourly);
            }
        }
    }
