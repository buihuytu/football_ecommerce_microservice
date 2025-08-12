using FluentValidation;
using Ordering.Application.Commands;

namespace Ordering.Application.Validators
{
    public class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>
    {
        public CheckoutOrderCommandValidator()
        {
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

            //RuleFor(o => o.Country).NotEmpty().WithMessage("Country is required.");
            //RuleFor(o => o.State).NotEmpty().WithMessage("State is required.");
            //RuleFor(o => o.ZipCode).NotEmpty().WithMessage("Zip code is required.");
            //RuleFor(o => o.CardName).NotEmpty().WithMessage("Card name is required.");
            //RuleFor(o => o.CardNumber).CreditCard().WithMessage("Invalid card number format.");
            //RuleFor(o => o.Expiration).Matches(@"^(0[1-9]|1[0-2])\/?([0-9]{4}|[0-9]{2})$").WithMessage("Invalid expiration date format (MM/YY or MM/YYYY expected).");
            //RuleFor(o => o.Cvv).Length(3, 4).WithMessage("CVV must be 3 or 4 digits long.");
        }
    }
}
