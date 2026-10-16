using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.SellerDashboard
{
    public class SellerDashboardOrdersRequestValidatore : AbstractValidator<SellerDashboardOrdersRequest>
    {
        public SellerDashboardOrdersRequestValidatore()
        {
            RuleFor(x => x.ToDate)
                .GreaterThanOrEqualTo(x => x.FromDate)
                .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
                .WithMessage("ToDate must be greater than or equal to FromDate.");
        }
    }
}
