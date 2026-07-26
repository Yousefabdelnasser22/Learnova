using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Constant;
using Learnova.Domain.Entities;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;

namespace Learnova.Application.Courses.Services
{
    using EnrollmentEntity = Learnova.Domain.Entities.Enrollment;

  
        public class CourseAccessService(IUnitOfWork unitOfWork) : ICourseAccessService
        {
            public async Task EnsureCanViewCourseContentAsync(
                int courseId,
                CurrentUser user,
                CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var course = await GetCourseOrThrowAsync(courseId, requirePublished: false);

                if (course.InstructorId == user.Id || user.IsInRole(UserRole.Admin))
                {
                    return;
                }

                EnsureCourseIsPublished(course);

                await EnsureActiveEnrollmentAsync(
                    courseId,
                    user.Id,
                    "You are not allowed to view this course content.");
            }

            public async Task EnsureStudentEnrolledInCourseAsync(
                int courseId,
                string userId,
                CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await GetCourseOrThrowAsync(courseId, requirePublished: true);

                await EnsureActiveEnrollmentAsync(
                    courseId,
                    userId,
                    "You are not enrolled in this course.");
            }

            public async Task EnsureInstructorOwnsCourseAsync(
                int courseId,
                string userId,
                CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await GetOwnedCourseOrThrowAsync(courseId, userId);
            }

            private async Task<Course> GetOwnedCourseOrThrowAsync(int courseId, string userId)
            {
                var course = (await unitOfWork.course.GetAllWithCondition(
                    course => course.Id == courseId && course.InstructorId == userId))
                    .FirstOrDefault();

                if (course is null)
                {
                    throw new NotFoundException("Course not found.");
                }

                return course;
            }

            private async Task<Course> GetCourseOrThrowAsync(int courseId, bool requirePublished)
            {
                var course = await unitOfWork.course.GetById(courseId);

                if (course is null)
                {
                    throw new NotFoundException("Course not found.");
                }

                if (requirePublished)
                {
                    EnsureCourseIsPublished(course);
                }

                return course;
            }

            private static void EnsureCourseIsPublished(Course course)
            {
                if (course.Status != CourseStatus.Published)
                {
                    throw new BadRequestException("Course is not available.");
                }
            }

            private async Task EnsureActiveEnrollmentAsync(
                int courseId,
                string userId,
                string errorMessage)
            {
                var enrollmentSpec = new ActiveEnrollmentByStudentAndCourseSpecification(userId, courseId);

                var enrollment = await unitOfWork.Repository<EnrollmentEntity>()
                    .GetEntityWithSpecAsync(enrollmentSpec);

                if (enrollment is null)
                {
                    throw new ForbiddenAccessException(errorMessage);
                }
            }
        }
    }
