using Application.DTOs.StoreDto;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IStoreService
    {
        Task<Result<StoreResponse>> CreateStore(CreateStoreRequest request);

        Task<SimpleResult> SendActivationRequest(Guid storeId);

        Task<Result<StoreResponse>> UpdateStore(UpdateStoreRequest request,Guid storeId);

        Task<SimpleResult> DeactivateStore(Guid storeId);

        Task<Result<List<StoreResponse>>> GetMyStores();

        Task<Result<StoreResponse>> GetStoreById(Guid storeId);
    }
}
