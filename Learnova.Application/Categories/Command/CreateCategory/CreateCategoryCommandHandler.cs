using Learnova.Application.Caching;
using Learnova.Application.Exceptions;
using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Categories.Command.CreateCategory
{
    public class CreateCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateCategoryCommandHandler> logger,
        ICacheInvalidationService cacheInvalidationService) : IRequestHandler<CreateCategoryCommand>
    {
        public async Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var name = request.Name.Trim();
            var existingCategories = await unitOfWork.category.GetAllWithCondition(c => c.Name == name);
            if (existingCategories.Any())
            {
                throw new ConflictException("Category with the same name already exists.");
            }

            var cateogry = new Category() { Name = name };

            await unitOfWork.category.Add(cateogry);
            await unitOfWork.CompleteAsync(cancellationToken);
            await cacheInvalidationService.EvictCategoriesAsync(cancellationToken);
            await cacheInvalidationService.EvictCoursesAsync(cancellationToken);

            logger.LogInformation("Category with name {Name} has been created successfully.", name);
        }
    }
}
