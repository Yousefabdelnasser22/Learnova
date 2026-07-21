using Learnova.Application.Caching;
using Learnova.Application.Exceptions;
using Learnova.Domain.Entites;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.SubCategories.Command.CreateSubCategory
{
    public class CreateSubCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateSubCategoryCommandHandler> _logger,
        ICacheInvalidationService cacheInvalidationService) : IRequestHandler<CreateSubCategoryCommand>
    {
        public async Task Handle(CreateSubCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting handling CreateSubCategoryCommand for CategoryId: {CategoryId}, Name: {Name}",
        request.CategoryId, request.Name);

            var category = await unitOfWork.category.GetById(request.CategoryId);
            if (category == null)
            {
                _logger.LogWarning("Category not found with Id: {CategoryId}", request.CategoryId);
                throw new NotFoundException("category not found");
            }
            var name = request.Name.Trim();
            var duplicateSubCategories = await unitOfWork.subCategory.GetAllWithCondition(
                s => s.CategoryId == request.CategoryId && s.Name == name);

            if (duplicateSubCategories.Any())
            {
                throw new ConflictException("SubCategory with the same name already exists in this category.");
            }

            var subCategory = new SubCategory() { Name = name, CategoryId = request.CategoryId };

            await unitOfWork.subCategory.Add(subCategory);
            await unitOfWork.CompleteAsync(cancellationToken);
            await cacheInvalidationService.EvictCategoriesAsync(cancellationToken);
            await cacheInvalidationService.EvictCoursesAsync(cancellationToken);

            _logger.LogInformation("SubCategory created successfully with Name: {Name} under CategoryId: {CategoryId}",
           name, request.CategoryId);

    }
    }
}
