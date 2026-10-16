using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SellerDashboardDto
{
    public class SellerDashboardRequest
    {
        public Guid StoreId { get; set; }
        public DateTime? FromDate { get; set; } = DateTime.UtcNow.AddDays(-30);
        public DateTime? ToDate { get; set; } = DateTime.UtcNow.Date;
    }

}
