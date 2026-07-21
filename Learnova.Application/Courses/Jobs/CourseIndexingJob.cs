using Learnova.Application.Common.BackgroundJobs;
using Learnova.Application.Courses.Services;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Courses.Jobs
{
    public sealed class CourseIndexingJob(
        IUnitOfWork unitOfWork,
        ICourseSearchService courseSearchService,
        ILogger<CourseIndexingJob> logger) : ICourseIndexingJob
    {
        public async Task IndexCourseAsync(int courseId)
        {
            logger.LogInformation("Course indexing job started for CourseId {CourseId}.", courseId);

            var course = await unitOfWork.course.GetById(courseId);

            if (course is null)
            {
                logger.LogWarning("Course indexing skipped. Course {CourseId} was not found.", courseId);
                return;
            }

            if (course.Status != CourseStatus.Published)
            {
                logger.LogWarning(
                    "Course indexing skipped. Course {CourseId} status is {Status}.",
                    course.Id,
                    course.Status);

                return;
            }

            await courseSearchService.IndexCourseAsync(course);

            logger.LogInformation("Course indexing job completed for CourseId {CourseId}.", courseId);
        }
    }
}
