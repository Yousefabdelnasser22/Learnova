using AutoMapper;
using Learnova.Application.Categories.DTO;
using Learnova.Application.Exceptions;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Categories.Query.GetCategoryById
{
    public class GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetCategoryByIdQueryHandler> logger) : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting category details for CategoryId: {CategoryId}", request.CategoryId);

            var category = await unitOfWork.category.GetById(request.CategoryId);

            if (category is null)
            {
                logger.LogWarning("Category not found. CategoryId: {CategoryId}", request.CategoryId);
                throw new NotFoundException("Category not found.");
            }

            return mapper.Map<CategoryDto>(category);
        }
    }
}
