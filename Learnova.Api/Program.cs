using Hangfire;
using Learnova.Api.Authorization;
using Learnova.Api.Extensions;
using Learnova.Api.Middlewares;
using Learnova.Api.Services;
using Learnova.Application.Caching;
using Learnova.Application.Extensions;
using Learnova.Domain.Entites;
using Learnova.Infrastructure.Data;
using Learnova.Infrastructure.Extensions;
using Learnova.Infrastructure.Search;
using Learnova.Infrastructure.Seeder;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

namespace Learnova.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddPresentation(builder.Configuration);
            builder.Services.AddApiRateLimiting();
            builder.Services.AddApplicationService();
            builder.Services.AddInfrastructureService(builder.Configuration, builder.Environment.EnvironmentName);
            builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 4;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.Configure<BearerTokenOptions>(
                IdentityConstants.BearerScheme,
                options =>
                {
                    options.BearerTokenExpiration = TimeSpan.FromMinutes(30);
                    options.RefreshTokenExpiration = TimeSpan.FromDays(7);
                });

            builder.Services
                .AddDataProtection()
                .SetApplicationName("Learnova.Api")
                .PersistKeysToDbContext<AppDbContext>();
            var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
            if (string.IsNullOrWhiteSpace(redisConnectionString))
            {
                throw new InvalidOperationException("Redis connection string is required.");
            }

            builder.Services.AddStackExchangeRedisOutputCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "Learnova:OutputCache:";
            });
            builder.Services.AddOutputCache(options =>
            {
                options.AddPolicy("Categories", policy =>
                    policy
                        .Expire(TimeSpan.FromMinutes(10))
                        .Tag("categories")
                        .SetVaryByQuery("search"));

                options.AddPolicy("CourseList", policy =>
                    policy
                        .Expire(TimeSpan.FromMinutes(1))
                        .Tag("courses")
                        .SetVaryByQuery(
                            "pageNumber",
                            "pageSize",
                            "search",
                            "categoryId",
                            "minPrice",
                            "maxPrice",
                            "subcategoryId",
                            "level",
                            "sort"));

                options.AddPolicy("CourseDetails", policy =>
                    policy
                        .Expire(TimeSpan.FromMinutes(5))
                        .Tag("courses"));
            });
            builder.Services.AddResilientOutputCacheStore();
            builder.Services.AddScoped<ICacheInvalidationService, OutputCacheInvalidationService>();

            builder.Host.UseSerilog((context, configuration) =>
             configuration
                  .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                  .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
                  .WriteTo.Console()
                  );

            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();

                var seeder = scope.ServiceProvider.GetRequiredService<Iseeder>();
                await seeder.seed();

                try
                {
                    var qdrantInitializer = scope.ServiceProvider.GetRequiredService<QdrantInitializer>();
                    await qdrantInitializer.InitializeAsync();
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Qdrant initialization failed. Semantic search will be unavailable until Qdrant is reachable.");
                }
            }


            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionMiddleware>();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(WebApplicationExtensions.FrontendCorsPolicy);
            app.UseAuthentication();
            app.UseRateLimiter();
            app.UseAuthorization();
            app.UseHangfireDashboard(
                "/hangfire",
                new DashboardOptions
                {
                    Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
                });
            app.UseRecurringJobs();
            app.UseOutputCache();
          
            app.MapControllers();

            app.MapGroup("/api/identity")
                .WithTags("identity")
                .RequireRateLimiting("auth-ip")
                .MapIdentityApi<ApplicationUser>();

            app.Run();
        }
    }
}
