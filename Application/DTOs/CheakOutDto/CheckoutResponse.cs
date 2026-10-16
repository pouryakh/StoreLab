using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CheakOutDto
{
    public class CheckoutResponse
    {
        public Guid OrderId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal RemainingBalance { get; set; }

        public PaymentStatus paymentStatus { get; set; }
    }
}
