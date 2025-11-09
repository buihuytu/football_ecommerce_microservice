using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Commands;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.Application.Handlers
{
    public class CheckoutOrderCommandHandlerV2 : IRequestHandler<CheckoutOrderCommandV2, int>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CheckoutOrderCommandHandlerV2> _logger;
        public CheckoutOrderCommandHandlerV2(IMapper mapper, IOrderRepository orderRepository, ILogger<CheckoutOrderCommandHandlerV2> logger)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _logger = logger;
        }
        public async Task<int> Handle(CheckoutOrderCommandV2 request, CancellationToken cancellationToken)
        {
            var order = _mapper.Map<Order>(request);
            await _orderRepository.AddAsync(order);
            _logger.LogInformation($"Order {order.Id} is successfully created.");
            return order.Id;
        }
    }
}
