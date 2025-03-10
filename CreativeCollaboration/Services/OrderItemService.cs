using Microsoft.EntityFrameworkCore;
using CreativeCollaboration.Data;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models;

namespace CreativeCollaboration.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly ApplicationDbContext _context;

        public OrderItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderItemDto>> ListOrderItems()
        {
            var orderItems = await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .ToListAsync();

            return orderItems.Select(oi => new OrderItemDto
            {
                OrderItemId = oi.OrderItemId,
                MenuItemName = oi.MenuItem?.MName ?? "Unknown",
                UnitPrice = oi.UnitOrderItemPrice,
                Quantity = oi.Quantity,
                TotalPrice = oi.TotalPrice
            }).ToList();
        }

        public async Task<ServiceResponse> DeleteOrderItem(int id)
        {
            ServiceResponse serviceResponse = new();

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Order item not found.");
                return serviceResponse;
            }

            try
            {
                _context.OrderItems.Remove(orderItem);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error deleting order item: " + ex.Message);
            }

            return serviceResponse;
        }
    }
}
