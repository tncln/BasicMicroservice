namespace Order.API.Models.ViewModels
{
    public class CreateOrderItemVM
    {
        public long TotalPrice { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public Guid ProductId { get; internal set; }
    } 
}
