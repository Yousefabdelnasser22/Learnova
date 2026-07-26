using Learnova.Application.Exceptions;
using Learnova.Application.Lesson.Specifications;
using Learnova.Application.Modules.Specifications;
using Learnova.Application.Quizzes.Specifications;
using Learnova.Domain.Entities;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Enrollment.Services
{
    public sealed class EnrollmentProgressService(
        IUnitOfWork unitOfWork,
        ILogger<EnrollmentProgressService> logger) : IEnrollmentProgressService
    {
        public async Task RecalculateModuleProgressAsync(
            string studentId,
            int moduleId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var lessonsByModuleSpec = new LessonsByModuleSpecification(moduleId);
            var totalLessons = await unitOfWork.Repository<Learnova.Domain.Entities.Lesson>()
                .CountWithSpecAsync(lessonsByModuleSpec);

            var completedLessonsSpec = new CompletedLessonProgressByModuleSpecification(
                studentId,
                moduleId);
            var completedLessons = await unitOfWork.Repository<LessonProgress>()
                .CountWithSpecAsync(completedLessonsSpec);

            var isCompleted = totalLessons > 0 && totalLessons == completedLessons;
            var moduleProgress = await unitOfWork.moduleProgress
                .CheckModuleProgressAsync(studentId, moduleId);

            if (moduleProgress is null)
            {
                moduleProgress = new ModuleProgress
                {
                    StudentId = studentId,
                    ModuleId = moduleId
                };

                await unitOfWork.moduleProgress.Add(moduleProgress);
            }

            moduleProgress.IsCompleted = isCompleted;
            moduleProgress.CompletedAt = isCompleted
                ? moduleProgress.CompletedAt ?? DateTime.UtcNow
                : null;

            logger.LogInformation(
                "Module progress recalculated for User {UserId} in Module {ModuleId}: {Completed}/{Total}, Completed={IsCompleted}",
                studentId,
                moduleId,
                completedLessons,
                totalLessons,
                isCompleted);
        }

        public async Task RecalculateCourseProgressAsync(
            string studentId,
            int courseId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var enrollment = await unitOfWork.enrollment
                .GetByStudentAndCourseAsync(studentId, courseId);

            if (enrollment is null)
            {
                throw new NotFoundException("Enrollment not found.");
            }

            var modulesByCourseSpec = new ModulesByCourseSpecification(courseId);
            var totalModules = await unitOfWork.Repository<Module>()
                .CountWithSpecAsync(modulesByCourseSpec);

            var completedModulesSpec = new CompletedModuleProgressByCourseSpecification(
                studentId,
                courseId);
            var completedModules = await unitOfWork.Repository<ModuleProgress>()
                .CountWithSpecAsync(completedModulesSpec);

            var courseQuizzes = (await unitOfWork.quiz
                .GetAllWithCondition(quiz => quiz.CourseId == courseId))
                .ToList();
            var passedQuizzes = 0;

            foreach (var quiz in courseQuizzes)
            {
                var passedQuizSpec = new PassedQuizAttemptSpecification(studentId, quiz.Id);
                if (await unitOfWork.Repository<QuizAttempt>().AnyWithSpecAsync(passedQuizSpec))
                {
                    passedQuizzes++;
                }
            }

            var totalRequiredItems = totalModules + courseQuizzes.Count;
            var completedRequiredItems = completedModules + passedQuizzes;
            var isCompleted = totalRequiredItems > 0 &&
                completedRequiredItems == totalRequiredItems;

            enrollment.ProgressPercentage = totalRequiredItems == 0
                ? 0
                : (int)((double)completedRequiredItems / totalRequiredItems * 100);
            enrollment.IsCompleted = isCompleted;
            enrollment.CompletedAt = isCompleted
                ? enrollment.CompletedAt ?? DateTime.UtcNow
                : null;

            if (isCompleted)
            {
                enrollment.Status = EnrollmentStatus.Completed;
            }
            else if (enrollment.Status == EnrollmentStatus.Completed)
            {
                enrollment.Status = EnrollmentStatus.Active;
            }

            logger.LogInformation(
                "Course progress recalculated for User {UserId} in Course {CourseId}: Modules {CompletedModules}/{TotalModules}, Quizzes {PassedQuizzes}/{TotalQuizzes}, Percentage={Percentage}, Completed={IsCompleted}",
                studentId,
                courseId,
                completedModules,
                totalModules,
                passedQuizzes,
                courseQuizzes.Count,
                enrollment.ProgressPercentage,
                isCompleted);
        }
    }
}
