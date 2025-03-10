namespace CreativeCollaboration.Models.ViewModels
{
    public class OrderItemDetails
    {
        public int OrderItemId { get; set; }
        public string MenuItemName { get; set; }
        public int Quantity { get; set; }
        public float UnitPrice { get; set; }
        public float TotalPrice { get; set; }
    }
}
