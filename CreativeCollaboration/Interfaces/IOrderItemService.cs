using CreativeCollaboration.Models;

namespace CreativeCollaboration.Interfaces
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItemDto>> ListOrderItems();
        Task<ServiceResponse> DeleteOrderItem(int id);
    }
}
