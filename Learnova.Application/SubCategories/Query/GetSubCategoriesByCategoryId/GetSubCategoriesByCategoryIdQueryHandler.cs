using AutoMapper;
using Learnova.Application.Exceptions;
using Learnova.Application.SubCategories.DTO;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.SubCategories.Query.GetSubCategoriesByCategoryId
{
    internal class GetSubCategoriesByCategoryIdQueryHandler(IMapper mapper , IUnitOfWork unitOfWork , ILogger<GetSubCategoriesByCategoryIdQueryHandler> _logger) : IRequestHandler<GetSubCategoriesByCategoryIdQuery, ICollection<SubCategoryDTO>>
    {
        public async Task<ICollection<SubCategoryDTO>> Handle(GetSubCategoriesByCategoryIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetSubCategoriesByCategoryIdQuery for CategoryId: {CategoryId}", request.CategoryId);

            var category = await unitOfWork.category.GetById(request.CategoryId);

            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found.", request.CategoryId);
                throw new NotFoundException($"Category with ID {request.CategoryId} not found.");
            }

            _logger.LogInformation("Category found: {CategoryName} (ID: {CategoryId})", category.Name, category.Id);

            var search = request.Search?.Trim();
            var subCategories = await unitOfWork.subCategory.GetAllWithCondition(
                s => s.CategoryId == request.CategoryId &&
                     (string.IsNullOrWhiteSpace(search) ||
                      s.Name.Contains(search) ||
                      s.Category.Name.Contains(search)),
                s => s.Category);

            if (subCategories == null || !subCategories.Any())
            {
                _logger.LogInformation("No subcategories found for Category ID {CategoryId}", request.CategoryId);
            }
            else
            {
                _logger.LogInformation("Found {Count} subcategories for Category ID {CategoryId}", subCategories.Count(), request.CategoryId);
            }

            var result = mapper.Map<ICollection<SubCategoryDTO>>(subCategories);

            _logger.LogInformation("Mapping to SubCategoryDTO completed for Category ID {CategoryId}", request.CategoryId);

            return result;


        }
    }
}
