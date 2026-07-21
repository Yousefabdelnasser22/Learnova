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

namespace Learnova.Application.Categories.Command.UpdateCategory
{
    public class UpdateCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateCategoryCommandHandler> logger,
        ICacheInvalidationService cacheInvalidationService) : IRequestHandler<UpdateCategoryCommand>
    {
        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting category update. CategoryId: {CategoryId}, Name: {Name}", request.CategoryId, request.Name);

            var category = await unitOfWork.category.GetById(request.CategoryId);

            if (category is null)
            {
                logger.LogWarning("Category not found. CategoryId: {CategoryId}", request.CategoryId);
                throw new NotFoundException("Category not found.");
            }

            var name = request.Name.Trim();
            var duplicateCategories = await unitOfWork.category.GetAllWithCondition(
                c => c.Id != request.CategoryId && c.Name == name);

            if (duplicateCategories.Any())
            {
                throw new ConflictException("Category with the same name already exists.");
            }

            category.Name = name;

            await unitOfWork.CompleteAsync(cancellationToken);
            await cacheInvalidationService.EvictCategoriesAsync(cancellationToken);
            await cacheInvalidationService.EvictCoursesAsync(cancellationToken);

            logger.LogInformation("Category updated successfully. CategoryId: {CategoryId}", request.CategoryId);
        }
    }
}
