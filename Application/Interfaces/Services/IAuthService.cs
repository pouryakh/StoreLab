using Application.DTOs.AuthDto;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request);

        Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request);

        Task<Result<AuthResponse>> RefreshAsync(RefreshRequest request);
    }
}
