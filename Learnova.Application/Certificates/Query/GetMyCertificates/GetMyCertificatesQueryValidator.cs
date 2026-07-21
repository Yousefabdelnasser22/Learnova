using FluentValidation;

namespace Learnova.Application.Certificates.Query.GetMyCertificates
{
    public class GetMyCertificatesQueryValidator : AbstractValidator<GetMyCertificatesQuery>
    {
        public GetMyCertificatesQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);

            RuleFor(x => x.Search)
                .MaximumLength(100);
        }
    }
}
