using FluentValidation;
using Learnova.Application.Behaviors;
using Learnova.Application.Common.BackgroundJobs;
using Learnova.Application.Courses.Jobs;
using Learnova.Application.Courses.Services;
using Learnova.Application.Enrollment.Services;
using Learnova.Application.Orders.Jobs;
using Learnova.Application.Quizzes.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Learnova.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static void AddApplicationService(this IServiceCollection service)
        {
            service.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            service.AddAutoMapper(typeof(ApplicationExtensions).Assembly);
            service.AddValidatorsFromAssembly(typeof(ApplicationExtensions).Assembly);
            service.AddScoped<ICourseAccessService, CourseAccessService>();
            service.AddScoped<ICourseContentChangeService, CourseContentChangeService>();
            service.AddScoped<IEnrollmentProgressService, EnrollmentProgressService>();
            service.AddScoped<IQuizAttemptInvalidationService, QuizAttemptInvalidationService>();
            service.AddScoped<ICourseIndexingJob, CourseIndexingJob>();
            service.AddScoped<IPendingOrderCleanupJob, PendingOrderCleanupJob>();
        }
    }
}
