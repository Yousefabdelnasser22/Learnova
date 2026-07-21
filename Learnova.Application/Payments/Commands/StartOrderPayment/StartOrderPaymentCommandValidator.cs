using FluentValidation;

namespace Learnova.Application.Payments.Commands.StartOrderPayment
{
    public sealed class StartOrderPaymentCommandValidator : AbstractValidator<StartOrderPaymentCommand>
    {
        public StartOrderPaymentCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("OrderId must be greater than 0");
        }
    }
}
