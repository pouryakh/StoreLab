using Application.DTOs.ReturnRequestDto;
using Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ReturnRequest
{
    public class CreateReturenReqeustValidator : AbstractValidator<CreateReturnRequestDto>
    {
        public CreateReturenReqeustValidator()
        {
            RuleFor(x => x.StoreOrderId)
            .NotEmpty()
            .WithMessage("Store order ID is required.");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Return reason is required.")
                .MaximumLength(1000)
                .WithMessage("Return reason cannot exceed 1000 characters.");
        }
    }
}
