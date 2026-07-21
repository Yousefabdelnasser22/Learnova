using AutoMapper;
using Learnova.Application.Exceptions;
using Learnova.Application.SubCategories.DTO;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.SubCategories.Query.GetSubCategoryById
{
    public class GetSubCategoryByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetSubCategoryByIdQueryHandler> logger) : IRequestHandler<GetSubCategoryByIdQuery, SubCategoryDTO>
    {
        public async Task<SubCategoryDTO> Handle(GetSubCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting subcategory details for SubCategoryId: {SubCategoryId}", request.SubCategoryId);

            var subCategory = await unitOfWork.subCategory.GetById(request.SubCategoryId, s => s.Category);

            if (subCategory is null)
            {
                logger.LogWarning("SubCategory not found. SubCategoryId: {SubCategoryId}", request.SubCategoryId);
                throw new NotFoundException("SubCategory not found.");
            }

            return mapper.Map<SubCategoryDTO>(subCategory);
        }
    }
}
