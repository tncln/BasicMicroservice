using MassTransit;
using Shared.Events;

namespace Payment.API.Consumer
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            
            //Ödeme işlemleri
            if(true)
            {
                //Ödeme Başarılı tamamlandı event süreç tamamlandı
                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderId = context.Message.OrderId
                };
                _publishEndpoint.Publish(paymentCompletedEvent);
            }
            else
            {
                //Ödeme Sıkıntı varsa event failed. 

                PaymentFailedEvent paymentFailedEvent = new()
                {
                     Message ="Bakiye Yetersiz",
                     OrderId = context.Message.OrderId
                };
                _publishEndpoint.Publish(paymentFailedEvent);
            }
            return Task.CompletedTask;
        }
    }
}
