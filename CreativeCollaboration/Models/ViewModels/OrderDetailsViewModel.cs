using System.Collections.Generic;

namespace CreativeCollaboration.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public DateOnly OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public float TotalOrderPrice { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
