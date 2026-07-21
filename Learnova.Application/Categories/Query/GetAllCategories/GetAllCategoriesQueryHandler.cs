using AutoMapper;
using Learnova.Application.Categories.DTO;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Categories.Query.GetAllCategories
{
    public class GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllCategoriesQueryHandler> logger) : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
    {
        public async Task<IEnumerable<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting all categories started.");

            var search = request.Search?.Trim();
            var categories = string.IsNullOrWhiteSpace(search)
                ? await unitOfWork.category.GetAll()
                : await unitOfWork.category.GetAllWithCondition(c => c.Name.Contains(search));

            var result = mapper.Map<IEnumerable<CategoryDto>>(categories);

            logger.LogInformation("GetAllCategoriesQuery completed successfully. Categories count: {Count}", result.Count());

            return result;
        }
    }
}
