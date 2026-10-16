using Application.Results;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<Result<Wallet>> PayOrderAsync(Order order);

        Task<SimpleResult> SettleStoreOrderAsync(StoreOrder storeOrder);

        Task<SimpleResult> RefundReturnRequestAsync(ReturnRequest returnRequest);

        Task<SimpleResult> RefundCancelOrderAsync(StoreOrder storeOrder, Guid userId);
    }
}
