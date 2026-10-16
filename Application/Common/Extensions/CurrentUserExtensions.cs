using Application.Interfaces.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Extensions
{
    public static class CurrentUserExtensions
    {
        public static Guid GetRequiredUserId(
            this ICurrentUserService currentUserService)
        {
            if (!currentUserService.IsAuthenticated ||
                currentUserService.UserId is null)
            {
                throw new UnauthorizedAccessException(
                    "Please login first.");
            }

            return currentUserService.UserId.Value;
        }
    }
}
