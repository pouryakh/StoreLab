using Application.DTOs.WalletDto;
using FluentValidation;

public class WalletPaginationRequestValidator
    : AbstractValidator<WalletPaginationRequest>
{
    public WalletPaginationRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 10);

        RuleFor(x => x.MinDate)
            .LessThanOrEqualTo(x => x.MaxDate)
            .When(x => x.MinDate.HasValue && x.MaxDate.HasValue)
            .WithMessage("MinDate must be less than or equal to MaxDate.");

        RuleFor(x => x.MinAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinAmount.HasValue)
            .WithMessage("MinAmount cannot be negative.");

        RuleFor(x => x.MaxAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxAmount.HasValue)
            .WithMessage("MaxAmount cannot be negative.");

        RuleFor(x => x.MinAmount)
            .LessThanOrEqualTo(x => x.MaxAmount)
            .When(x => x.MinAmount.HasValue && x.MaxAmount.HasValue)
            .WithMessage("MinAmount must be less than or equal to MaxAmount.");
    }
}