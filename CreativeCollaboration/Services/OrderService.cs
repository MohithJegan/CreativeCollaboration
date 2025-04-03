using Microsoft.EntityFrameworkCore;
using CreativeCollaboration.Data;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models;
using Microsoft.AspNetCore.Identity;

namespace CreativeCollaboration.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IEnumerable<OrderDto>> ListOrders()
        {
            // Get the logged-in user
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (user == null)
            {
                return new List<OrderDto>(); // Return an empty list if the user is not found
            }

            // Check if the user is a customer
            bool isCustomer = await _userManager.IsInRoleAsync(user, "Customer");

            // Fetch orders conditionally based on user role
            var orders = await _context.Orders
                .Include(o => o.Customer)        // Includes Customer details
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem) // Includes MenuItem inside OrderItems
                .Where(o => !isCustomer || o.CustomerAccountId == user.Id) // Only filter if user is a customer
                .ToListAsync();

            // Convert orders to DTOs
            return orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                CustomerName = o.Customer?.Name ?? "Unknown",
                TotalOrderPrice = o.OrderItems.Sum(oi => oi.TotalPrice),
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    MenuItemName = oi.MenuItem?.MName ?? "Unknown",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitOrderItemPrice,
                    TotalPrice = oi.TotalPrice
                }).ToList()
            }).ToList();
        }





        public async Task<IEnumerable<OrderDto>> ListOrdersByCustomerId(int customerId)
        {
            var orders = await _context.Orders
                .Where(o => o.CustomerId == customerId) // Filters orders for the specific customer
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem) 
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                LastOrderPrice = o.OrderItems.Sum(oi => (decimal)oi.TotalPrice),
                LastOrderMenuNames = o.OrderItems.Select(oi => oi.MenuItem.MName).ToList()
            }).ToList();
        }

        public async Task<OrderDto?> FindOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return null;
            }

            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId ?? 0,
                CustomerName = order.Customer?.Name ?? "Unknown",
                TotalOrderPrice = order.OrderItems.Sum(oi => oi.TotalPrice),
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    MenuItemId = oi.MenuItemId,  // ✅ Ensure MenuItemId is included
                    MenuItemName = oi.MenuItem?.MName ?? "Unknown",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitOrderItemPrice,
                    TotalPrice = oi.Quantity * oi.UnitOrderItemPrice
                }).ToList()

            };
        }

        public async Task<ServiceResponse> AddOrder(AUOrderDto orderDto)
        {
            ServiceResponse serviceResponse = new();

            // ✅ Fetch the Customer from DB using CustomerId
            var customer = await _context.Customers.FindAsync(orderDto.CustomerId);
            if (customer == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Customer not found.");
                return serviceResponse;
            }

            // ✅ Create a new Order
            Order order = new()
            {
                OrderDate = orderDto.OrderDate,
                CustomerId = orderDto.CustomerId,
                Customer = customer
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Save order first to generate Order ID

            float totalOrderPrice = 0;

            foreach (var orderItemDto in orderDto.OrderItems)
            {
                // ✅ Fetch Menu Item Price
                var menuItem = await _context.MenuItems.FindAsync(orderItemDto.MenuItemId);
                if (menuItem == null)
                {
                    serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                    serviceResponse.Messages.Add($"Menu item ID {orderItemDto.MenuItemId} not found.");
                    return serviceResponse;
                }

                float unitPrice = menuItem.Price;
                float totalPrice = orderItemDto.Quantity * unitPrice;
                totalOrderPrice += totalPrice;

                // ✅ Create Order Item and Assign Prices
                OrderItem orderItem = new()
                {
                    OrderId = order.OrderId,
                    MenuItemId = orderItemDto.MenuItemId,
                    Quantity = orderItemDto.Quantity,
                    UnitOrderItemPrice = unitPrice,
                    TotalPrice = totalPrice
                };

                _context.OrderItems.Add(orderItem);
            }

            // ✅ Save all order items to database
            await _context.SaveChangesAsync();

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = order.OrderId;
            return serviceResponse;
        }



        public async Task<ServiceResponse> DeleteOrder(int id)
        {
            ServiceResponse serviceResponse = new();

            // ✅ Find the order
            var order = await _context.Orders
                .Include(o => o.OrderItems) // Load related order items
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Order not found.");
                return serviceResponse;
            }

            try
            {
                // ✅ First, remove related order items
                _context.OrderItems.RemoveRange(order.OrderItems);

                // ✅ Then, remove the order itself
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error deleting order: " + ex.Message);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse> UpdateOrder(int id, AUOrderDto updateOrderDto)
        {
            ServiceResponse serviceResponse = new();

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Order not found.");
                return serviceResponse;
            }

            // ✅ Update Order Details (Date & Customer)
            order.OrderDate = updateOrderDto.OrderDate;
            order.CustomerId = updateOrderDto.CustomerId;

            // ✅ Update or Add Order Items
            foreach (var updatedItem in updateOrderDto.OrderItems)
            {
                var existingItem = order.OrderItems.FirstOrDefault(oi => oi.OrderItemId == updatedItem.OrderItemId);

                if (existingItem != null)
                {
                    // Update existing item
                    existingItem.Quantity = updatedItem.Quantity;
                    existingItem.UnitOrderItemPrice = updatedItem.UnitOrderItemPrice;
                    existingItem.TotalPrice = updatedItem.Quantity * updatedItem.UnitOrderItemPrice;
                }
                else
                {
                    // Fetch Menu Item Price for New Item
                    var menuItem = await _context.MenuItems.FindAsync(updatedItem.MenuItemId);
                    if (menuItem == null)
                    {
                        serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                        serviceResponse.Messages.Add($"Menu item with ID {updatedItem.MenuItemId} not found.");
                        return serviceResponse;
                    }

                    // ✅ Add new order item
                    var newItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        MenuItemId = updatedItem.MenuItemId,
                        Quantity = updatedItem.Quantity,
                        UnitOrderItemPrice = menuItem.Price,
                        TotalPrice = updatedItem.Quantity * menuItem.Price
                    };

                    order.OrderItems.Add(newItem);
                }
            }

            // ✅ Remove deleted items (items that exist in DB but not in updated list)
            var updatedItemIds = updateOrderDto.OrderItems.Select(oi => oi.OrderItemId).ToList();
            var itemsToRemove = order.OrderItems.Where(oi => !updatedItemIds.Contains(oi.OrderItemId)).ToList();

            foreach (var item in itemsToRemove)
            {
                _context.OrderItems.Remove(item);
            }

            try
            {
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error updating order: " + ex.Message);
            }

            return serviceResponse;
        }



        public async Task<IEnumerable<CustomerDto>> ListCustomers()
        {
            return await _context.Customers
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItemDto>> ListMenuItems()
        {
            return await _context.MenuItems
                .Select(m => new MenuItemDto
                {
                    MenuItemId = m.MenuItemId,
                    MName = m.MName,
                    Price = m.Price
                })
                .ToListAsync();
        }


    }
}
