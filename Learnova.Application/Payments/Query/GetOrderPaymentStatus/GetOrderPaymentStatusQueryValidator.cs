using FluentValidation;

namespace Learnova.Application.Payments.Query.GetOrderPaymentStatus
{
    public sealed class GetOrderPaymentStatusQueryValidator : AbstractValidator<GetOrderPaymentStatusQuery>
    {
        public GetOrderPaymentStatusQueryValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("OrderId must be greater than 0");
        }
    }
}
