using Discount.Application.Queries;
using Discount.Core.Repositories;
using Discount.Grpc.Protos;
using Grpc.Core;
using MediatR;

namespace Discount.Application.Handlers
{
    public class GetDiscountByProductNameHandler : IRequestHandler<GetDiscountByProductNameQuery, CouponModel>
    {
        private readonly IDiscountRepository _discountRepository;
        public GetDiscountByProductNameHandler(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<CouponModel> Handle(GetDiscountByProductNameQuery request, CancellationToken cancellationToken)
        {
            var coupon = await _discountRepository.GetDiscountByProductName(request.ProductName);
            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount for the Product name = {request.ProductName} not found"));
            }
            var couponModel = new CouponModel
            {
                Id = coupon.Id,
                ProductName = coupon.ProductName,
                Description = coupon.Description,
                Amount = coupon.Amount
            };
            return couponModel;
        }
    }
}
