using Application.DTOs.StoreDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Store
{
    public class UpdateStoreRequestValidator : AbstractValidator<UpdateStoreRequest>
    {
        public UpdateStoreRequestValidator()
        {
            RuleFor(x => x)
                .Custom((request, context) =>
                {
                    var hasName =
                        !string.IsNullOrWhiteSpace(request.Name);

                    var hasDescription =
                        !string.IsNullOrWhiteSpace(request.Description);

                    if (!hasName && !hasDescription)
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
