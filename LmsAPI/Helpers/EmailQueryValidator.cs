using FluentValidation;

namespace LMSAPI.Helpers
{
    public class EmailQueryValidator: AbstractValidator<string>
    {
        public EmailQueryValidator()
        {
            RuleFor(x => x)
                 .NotEmpty().WithMessage("Email is required")
                 .NotNull().WithMessage("Email is required")
                 .EmailAddress().WithMessage("Please enter a valid email address");
        }
    }
}
