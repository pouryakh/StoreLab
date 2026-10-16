using FluentValidation;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.FirstName) ||
                !string.IsNullOrWhiteSpace(x.LastName) ||
                !string.IsNullOrWhiteSpace(x.Email) ||
                !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage(
                "At least one field must be provided.");

        RuleFor(x => x.FirstName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Please enter a valid email address.");

        RuleFor(x => x.Email)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));  

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^09\d{9}$")
            .WithMessage("Phone number must be a valid Iranian mobile number.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
    }
}