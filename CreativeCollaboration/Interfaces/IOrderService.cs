using CreativeCollaboration.Models;

namespace CreativeCollaboration.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> ListOrders();
        Task<IEnumerable<OrderDto>> ListOrdersByCustomerId(int customerId);
        Task<OrderDto?> FindOrder(int id);
        Task<ServiceResponse> AddOrder(AUOrderDto orderDto);
        Task<ServiceResponse> DeleteOrder(int id);

        Task<ServiceResponse> UpdateOrder(int id, AUOrderDto updateOrderDto);

        Task<IEnumerable<CustomerDto>> ListCustomers();
        Task<IEnumerable<MenuItemDto>> ListMenuItems();
    }
}
