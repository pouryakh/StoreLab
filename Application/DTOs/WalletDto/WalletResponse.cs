using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.WalletDto
{
    public class WalletResponse
    {
        public Guid WalletId { get; set; }
        public Guid? UserId { get; set; }

        public WalletType Type { get; set; }

        public decimal Balance { get; set; }
    }
}
