using Application.DTOs.ProductDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Product
{
    public class ProductRequestValidator : AbstractValidator<CreateProductRequestDto>
    {
        public ProductRequestValidator()
        {

            RuleFor(x => x.Price).NotEmpty().GreaterThan(0);
            RuleFor(X => X.Name).NotEmpty();
            RuleFor(X => X.Description).NotEmpty();
            RuleFor(X => X.StockQuantity).GreaterThan(0);
        }
    }
}
