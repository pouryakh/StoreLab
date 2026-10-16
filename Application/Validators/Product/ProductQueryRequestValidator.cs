using Application.DTOs.ProductDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Product
{
    public class ProductQueryRequestValidator : AbstractValidator<ProductQueryRequest>
    {
        public ProductQueryRequestValidator()
        {
            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 10)
                .WithMessage("Page size must be between 1 and 10.");

            RuleFor(x => x.MinPrice)
                .GreaterThan(0)
                .When(x => x.MinPrice.HasValue)
                .WithMessage("Minimum price must be greater than zero.");

            RuleFor(x => x.MaxPrice)
                .GreaterThan(0)
                .When(x => x.MaxPrice.HasValue)
                .WithMessage("Maximum price must be greater than zero.");

            RuleFor(x => x.MinPrice)
                .LessThanOrEqualTo(x => x.MaxPrice)
                .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue)
                .WithMessage("Minimum price cannot be greater than maximum price.");

        }
    }
}
