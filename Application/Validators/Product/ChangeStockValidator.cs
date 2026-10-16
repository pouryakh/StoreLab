using Application.DTOs.ProductDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Product
{
    public class ChangeStockValidator : AbstractValidator<ChangeStockRequest>
    {
        public ChangeStockValidator()
        {
            RuleFor(x => x.NewStock).GreaterThanOrEqualTo(0).NotEmpty();

            RuleFor(x => x.ProductId).NotNull();
        }
    }
}
