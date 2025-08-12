using Discount.Grpc.Protos;
using MediatR;

namespace Discount.Application.Queries
{
    public class GetDiscountByProductNameQuery : IRequest<CouponModel>
    {
        public string ProductName { get; set; }
        public GetDiscountByProductNameQuery(string productName)
        {
            ProductName = productName;
        }
    }
}
