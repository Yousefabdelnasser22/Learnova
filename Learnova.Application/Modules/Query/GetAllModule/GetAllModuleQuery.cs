using Learnova.Application.Common.Queries;
using Learnova.Application.Modules.DTO;
using MediatR;

namespace Learnova.Application.Modules.Query.GetAllModule
{
    public class GetAllModuleQuery : PagedSearchQuery, IRequest<List<ModuleDTO>>
    {
        public int CourseId { get; set; }
    }
}
