namespace CreativeCollaboration.Models.ViewModels
{
    public class OrderDetails
    {
        public int OrderId { get; set; }
        public DateOnly OrderDate { get; set; }
        public string CustomerName { get; set; }
        public float TotalOrderPrice { get; set; }
        public List<OrderItemDetails> OrderItems { get; set; } = new();
    }
}
