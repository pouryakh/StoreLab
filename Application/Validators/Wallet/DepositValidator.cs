using Application.DTOs.WalletDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Wallet
{
    public class DepositValidator : AbstractValidator<DepositRequest>
    {
        public DepositValidator()
        {
            RuleFor(x => x.Amount).NotEmpty().GreaterThan(0);

            RuleFor(x => x.Description).MaximumLength(500);
        }
    }
}
