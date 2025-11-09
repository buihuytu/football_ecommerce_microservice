using Discount.Application.Queries;
using Discount.Core.Repositories;
using Discount.Grpc.Protos;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Discount.Application.Handlers
{
    public class GetDiscountByProductNameHandler : IRequestHandler<GetDiscountByProductNameQuery, CouponModel>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly ILogger<GetDiscountByProductNameHandler> _logger;
        public GetDiscountByProductNameHandler(IDiscountRepository discountRepository, ILogger<GetDiscountByProductNameHandler> logger)
        {
            _discountRepository = discountRepository;
            _logger = logger;
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
            _logger.LogInformation($"Coupon for Product name: {request.ProductName} has fetched.");
            return couponModel;
        }
    }
}
