using Application.DTOs.ShoppingCartDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ShoppingCart
{
    public class UpdateItemQuantityValidator : AbstractValidator<UpdateItemQuantityRequest>
    {
        public UpdateItemQuantityValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();

            RuleFor(x => x.NewQuantity).NotEmpty().GreaterThanOrEqualTo(0);
        }
    }
}
