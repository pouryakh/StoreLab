using Application.DTOs.ShoppingCartDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ShoppingCart
{
    public class AddItemValidator : AbstractValidator<AddItemRequest>
    {
        public AddItemValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();

            RuleFor(x => x.Quantity).NotEmpty().GreaterThan(0);
        }
    }
}
