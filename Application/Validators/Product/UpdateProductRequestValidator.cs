using Application.DTOs.ProductDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Product
{
    public class UpdateProductRequestValidator
    : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(x => x)
                .Custom((request, context) =>
                {
                    var hasName =
                        !string.IsNullOrWhiteSpace(request.Name);

                    var hasDescription =
                        !string.IsNullOrWhiteSpace(request.Description);

                    var hasPrice =
                        request.Price.HasValue && request.Price != 0;

                    if (!hasName && !hasDescription && !hasPrice)
                    {
                        context.AddFailure(
                            "At least one field must be provided.");
                    }
                });

            RuleFor(x => x.Name)
                .MaximumLength(250)
                .When(x => !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
