using Learnova.Application.Caching;
using Learnova.Application.Exceptions;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Categories.Command.DeleteCategory
{
    public class DeleteCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteCategoryCommandHandler> logger,
        ICacheInvalidationService cacheInvalidationService) : IRequestHandler<DeleteCategoryCommand>
    {
        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting category with id: {CategoryId}", request.CategoryId);

            var category = await unitOfWork.category.GetById(request.CategoryId);

            if (category is null)
            {
                logger.LogWarning("Category not found. CategoryId: {CategoryId}", request.CategoryId);
                throw new NotFoundException("Category not found.");
            }

            var subCategories = await unitOfWork.subCategory.GetAllWithCondition(s => s.CategoryId == request.CategoryId);

            if (subCategories.Any())
            {
                logger.LogWarning("Category with id {CategoryId} cannot be deleted because it has subcategories.", request.CategoryId);
                throw new BadRequestException("Category cannot be deleted because it has subcategories.");
            }

            await unitOfWork.category.Delete(request.CategoryId);
            await unitOfWork.CompleteAsync(cancellationToken);
            await cacheInvalidationService.EvictCategoriesAsync(cancellationToken);
            await cacheInvalidationService.EvictCoursesAsync(cancellationToken);

            logger.LogInformation("Category deleted successfully. CategoryId: {CategoryId}", request.CategoryId);
        }
    }
}
