﻿using FluentValidation;
using Ordering.Application.Commands;

namespace Ordering.Application.Validators
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(o => o.Id)
                .NotEmpty()
                .WithMessage("Id is required.")
                .GreaterThan(0)
                .WithMessage("Order ID must be greater than zero.");

            RuleFor(o => o.UserName)
                .NotEmpty()
                .WithMessage("Username is required.")
                .MaximumLength(70)
                .WithMessage("Username must not exceed 70 characters");

            RuleFor(o => o.TotalPrice)
                .NotEmpty()
                .WithMessage("TotalPrice is required.")
                .GreaterThan(-1)
                .WithMessage("Total price should not be less than zero.");

            RuleFor(o => o.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.");

            RuleFor(o => o.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.");

            RuleFor(o => o.EmailAddress)
                .NotEmpty()
                .WithMessage("Email address is required.")
                .EmailAddress()
                .WithMessage("Invalid email address format.");
        }
    }
}
