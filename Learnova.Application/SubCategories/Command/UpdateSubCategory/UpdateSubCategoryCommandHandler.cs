using Learnova.Application.Caching;
using Learnova.Application.Exceptions;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.SubCategories.Command.UpdateSubCategory
{
    public class UpdateSubCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateSubCategoryCommandHandler> logger,
        ICacheInvalidationService cacheInvalidationService) : IRequestHandler<UpdateSubCategoryCommand>
    {
        public async Task Handle(UpdateSubCategoryCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Starting subcategory update. SubCategoryId: {SubCategoryId}, Name: {Name}",
                request.SubCategoryId,
                request.Name);

            var subCategory = await unitOfWork.subCategory.GetById(request.SubCategoryId);

            if (subCategory is null)
            {
                logger.LogWarning("SubCategory not found. SubCategoryId: {SubCategoryId}", request.SubCategoryId);
                throw new NotFoundException("SubCategory not found.");
            }

            var name = request.Name.Trim();
            var duplicateSubCategories = await unitOfWork.subCategory.GetAllWithCondition(
                s => s.Id != request.SubCategoryId &&
                     s.CategoryId == subCategory.CategoryId &&
                     s.Name == name);

            if (duplicateSubCategories.Any())
            {
                throw new ConflictException("SubCategory with the same name already exists in this category.");
            }

            subCategory.Name = name;

            await unitOfWork.CompleteAsync(cancellationToken);
            await cacheInvalidationService.EvictCategoriesAsync(cancellationToken);
            await cacheInvalidationService.EvictCoursesAsync(cancellationToken);

            logger.LogInformation("SubCategory updated successfully. SubCategoryId: {SubCategoryId}", request.SubCategoryId);
        }
    }
}
