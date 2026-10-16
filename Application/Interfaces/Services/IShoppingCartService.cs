using Application.DTOs.ShoppingCartDto;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IShoppingCartService
    {
        Task<SimpleResult> AddItem(AddItemRequest request);

        Task<SimpleResult> UpdateItemQuantity(UpdateItemQuantityRequest request);

        Task<SimpleResult> ClearShoppingCart();

        Task<SimpleResult> RemoveItem(RemoveItemRequest request);

        Task<Result<ShoppingCartResponse>> GetShoppingCart();
    }
}
