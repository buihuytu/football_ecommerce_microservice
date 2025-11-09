using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Ordering.Application.Commands;

namespace Ordering.API.EventBusConsumer
{
    public class BasketOrderingConsumerV2 : IConsumer<BasketCheckoutEventV2>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<BasketOrderingConsumerV2> _logger;
        public BasketOrderingConsumerV2(IMediator mediator, IMapper mapper, ILogger<BasketOrderingConsumerV2> logger)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<BasketCheckoutEventV2> context)
        {
            using var scope = _logger.BeginScope("Consumer Basket Checkout Event for {correlationId}", context.Message.CorrelationId);
            var command = _mapper.Map<CheckoutOrderCommandV2>(context.Message);
            var result = await _mediator.Send(command);
            _logger.LogInformation($"Basket Checkout Event consumed successfully for User: {command.UserName}");
        }
    }
}
