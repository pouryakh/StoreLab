using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<UserResponse>> GetUser();

        Task<Result<UserResponse>> UpdateUser(UpdateUserRequest request);

        Task<Result<UserResponse>> UpdateUserPassword(UpdatePasswordRequest request);
    }
}
