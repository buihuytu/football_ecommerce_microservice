using MediatR;

namespace Discount.Application.Commands
{
    public class DeleteDiscountByProductNameCommand : IRequest<bool>
    {
        public string ProductName { get; set; }
        public DeleteDiscountByProductNameCommand(string productName)
        {
            ProductName = productName;
        }
    }
}
