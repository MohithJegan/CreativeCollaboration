namespace CreativeCollaboration.Models.ViewModels
{
    public class MenuItemDetails
    {
        public int MenuItemId { get; set; }
        public MenuItemDto MenuItem { get; set; }
        public string MName { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }

        public List<OrderDto>? ListOrders { get; set; }
    }
}
