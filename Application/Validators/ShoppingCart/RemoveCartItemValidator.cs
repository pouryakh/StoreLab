using Application.DTOs.ShoppingCartDto;
using FluentValidation;

namespace Application.Validators.ShoppingCart
{
    public class RemoveItemRequestValidator
        : AbstractValidator<RemoveItemRequest>
    {
        public RemoveItemRequestValidator()
        {
            RuleFor(x => x.CartItemId)
                .NotEmpty()
                .WithMessage("Cart item id is required.");
        }
    }
}
