using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Stock.API.Models.Entities;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        readonly MongoDBService _mongoDBService;
        IMongoCollection<Stock.API.Models.Entities.Stock> _stockCollection;
        readonly ISendEndpointProvider _sendEndpointProvider;
        public OrderCreatedEventConsumer(MongoDBService mongoDBService, ISendEndpointProvider sendEndpointProvider)
        {
            _mongoDBService = mongoDBService;
            _stockCollection = mongoDBService.GetCollection<Stock.API.Models.Entities.Stock>();
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            foreach (var item in context.Message.OrderItems)
            {
               stockResult.Add((await _stockCollection.FindAsync(s => s.ProductId == item.ProductId && s.Count >= item.Count)).Any());
            }
            if(stockResult.TrueForAll(x=> x.Equals(true)))
            {
                foreach (var orderItem in context.Message.OrderItems)
                {
                   Stock.API.Models.Entities.Stock stock = await (await _stockCollection.FindAsync(x => x.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();

                    stock.Count -= orderItem.Count;
                    await _stockCollection.FindOneAndReplaceAsync(x => x.ProductId == orderItem.ProductId, stock);
                }
                //Payment a event gönderilir olumlu 
                StockReservedEvent stockReservedEvent = new StockReservedEvent()
                {
                    BuyerId= context.Message.BuyerId,
                    OrderId= context.Message.OrderId,
                    TotalPrice= context.Message.TotalPrice,
                };
                ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue :{RabbitMQSettings.Stock_OrderCreatedEventQueue}"));
                await sendEndpoint.Send(stockReservedEvent);
            }
            else
            {
                //sipariş geçersiz
            }
            return Task.CompletedTask;
        }
    }
}
