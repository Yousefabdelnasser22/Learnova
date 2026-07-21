using Learnova.Application.Caching;
using Learnova.Application.Exceptions;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.SubCategories.Command.DeleteSubCategory
{
    public class DeleteSubCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteSubCategoryCommandHandler> logger,
        ICacheInvalidationService cacheInvalidationService) : IRequestHandler<DeleteSubCategoryCommand>
    {
        public async Task Handle(DeleteSubCategoryCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting subcategory with id: {SubCategoryId}", request.SubCategoryId);

            var subCategory = await unitOfWork.subCategory.GetById(request.SubCategoryId, s => s.Courses);

            if (subCategory is null)
            {
                logger.LogWarning("SubCategory not found. SubCategoryId: {SubCategoryId}", request.SubCategoryId);
                throw new NotFoundException("SubCategory not found.");
            }

            if (subCategory.Courses.Any(course => !course.IsDeleted))
            {
                logger.LogWarning(
                    "SubCategory with id {SubCategoryId} cannot be deleted because it has courses.",
                    request.SubCategoryId);
                throw new BadRequestException("SubCategory cannot be deleted because it has courses.");
            }

            await unitOfWork.subCategory.Delete(request.SubCategoryId);
            await unitOfWork.CompleteAsync(cancellationToken);
            await cacheInvalidationService.EvictCategoriesAsync(cancellationToken);
            await cacheInvalidationService.EvictCoursesAsync(cancellationToken);

            logger.LogInformation("SubCategory deleted successfully. SubCategoryId: {SubCategoryId}", request.SubCategoryId);
        }
    }
}
