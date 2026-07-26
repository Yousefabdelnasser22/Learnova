using Learnova.Domain.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Learnova.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
    {
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> lessons { get; set; }

        public DbSet<Module> Modules { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<LessonProgress> LessonProgress { get; set; }
        public DbSet<ModuleProgress> ModuleProgress { get; set; }

        public DbSet<Quiz> Quiz { get; set; }

        public DbSet<QuizAnswer> QuizAnswers { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }

        public DbSet<Certificate> Certificates { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<FavoriteList> FavoriteList { get; set; }

        public DbSet<ProcessedWebhookEvent> ProcessedWebhookEvents { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            ApplySoftDeleteQueryFilters(builder);
        }

        private static void ApplySoftDeleteQueryFilters(ModelBuilder builder)
        {
            var entityTypes = builder.Model
                .GetEntityTypes()
                .Where(entityType =>
                    typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) &&
                    entityType.BaseType is null);

            foreach (var entityType in entityTypes)
            {
                var parameter = Expression.Parameter(entityType.ClrType, "entity");
                var isDeletedProperty = Expression.Property(
                    parameter,
                    nameof(BaseEntity.IsDeleted));

                var filter = Expression.Lambda(
                    Expression.Not(isDeletedProperty),
                    parameter);

                entityType.SetQueryFilter(filter);
            }
        }
    }
}
