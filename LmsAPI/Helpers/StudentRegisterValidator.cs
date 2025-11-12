using FluentValidation;
using LMSAPI.DTO;

namespace LMSAPI.Helpers
{
    public class StudentRegisterValidator: AbstractValidator<StudentRegisterDto>
    {
        public StudentRegisterValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Full name is required.")
                .Length(3, 100).WithMessage("Full name must be between 3 and 100 characters.");
                //.Matches(@"^[a-zA-Z\s]+$").WithMessage("Full name must contain only letters.");

            RuleFor(x => x.EmailId)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Invalid email format.")
               .MaximumLength(200).WithMessage("Email cannot exceed 200 characters.");

            RuleFor(x => x.Mobile)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^[0-9]{8,15}$").WithMessage("Phone number must contain only digits (8-15 characters).");

            // Country Code
            RuleFor(x => x.CountryCode)
                .NotEmpty().WithMessage("Country code is required.")
                .Matches(@"^\+\d{1,4}$").WithMessage("Country code must start with '+' followed by digits (e.g. +91).");

            // DeviceMacId
            RuleFor(x => x.DeviceMacId)
                .NotEmpty().WithMessage("Device MAC ID is required.")
                .Length(10, 100).WithMessage("Invalid Device ID format.");
        }
    }
}
