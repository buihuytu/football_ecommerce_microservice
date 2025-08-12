using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Commands;
using Ordering.Application.Exceptions;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.Application.Handlers
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger _logger;
        public DeleteOrderCommandHandler(IOrderRepository orderRepository, ILogger logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }
        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id);
            if(order == null)
            {
                throw new OrderNotFoundException(nameof(Order), request.Id);
            }
            await _orderRepository.DeleteAsync(order);
            _logger.LogInformation($"Order with id {request.Id} is successfully deleted.");
            return Unit.Value;
        }
    }
}
