using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    static public class RabbitMQSettings
    {
        public const string Stock_OrderCreatedEventQueue = "stock-order-created-event-queue";
        public const string Payment_StockReservedEventQueue = "Stock_Event_Queue";
        public const string Payment_StockConsumer = "Stock_Consumer";
        public const string Order_Payment_Completed_Event_Queue = "Order_Payment_Completed_Event_Queue";
        public const string Stock_Consumer = "Stock_Consumer";
        public const string Payment_Failed = "Payment_Failed";
    }
}
