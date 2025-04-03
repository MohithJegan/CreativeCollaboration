using Microsoft.EntityFrameworkCore;
using CreativeCollaboration.Data;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace CreativeCollaboration.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderItemService(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
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

        //public async Task<IEnumerable<OrderItemDto>> ListOrderItems()
        //{
        //    IdentityUser? Users = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        //    List<OrderItem> orderItems = await _context.OrderItems
        //        .Include(oi=> oi.Order)
        //        .Include(oi => oi.MenuItem)
        //        .ToListAsync();
        //    List<Customer> customers = await _context.Customers
        //        .Include(c => c.Orders)
        //        .ThenInclude(o => o.OrderItems)
        //        .ToListAsync(); // ✅ Convert to List first

        //    List<CustomerDto> customerDtos = new();
        //    List<OrderItemDto> Orders = new();

        //    var orderList =  orderItems.Select(oi => new OrderItemDto
        //    {
        //        OrderItemId = oi.OrderItemId,
        //        MenuItemName = oi.MenuItem?.MName ?? "Unknown",
        //        UnitPrice = oi.UnitOrderItemPrice,
        //        Quantity = oi.Quantity,
        //        TotalPrice = oi.TotalPrice
        //    }).ToList();


        //    if (Users != null)
        //    {
        //        // Admin to see all the customers
        //        if (Users.Email == "mohith@test.ca")
        //        {

        //            return orderItems.Select(oi => new OrderItemDto
        //            {
        //                OrderItemId = oi.OrderItemId,
        //                MenuItemName = oi.MenuItem?.MName ?? "Unknown",
        //                UnitPrice = oi.UnitOrderItemPrice,
        //                Quantity = oi.Quantity,
        //                TotalPrice = oi.TotalPrice
        //            }).ToList();
        //        }

        //        else
        //        {
        //            // customer to see only their profile
        //            foreach (Customer customer in customers)
        //            {
        //                if (customer.Email == Users.Email)
        //                {
        //                    foreach (Order CustomerOrders in customer.Orders)
        //                    {
        //                        if(CustomerOrders.CustomerAccountId == customer.CustomerAccountId)
        //                        {
        //                           // Orders.Add()

        //                        }
        //                    }
        //                }
        //                //return orderItems.Select(oi => new OrderItemDto
        //                //{
        //                //    OrderItemId = oi.OrderItemId,
        //                //    MenuItemName = oi.MenuItem?.MName ?? "Unknown",
        //                //    UnitPrice = oi.UnitOrderItemPrice,
        //                //    Quantity = oi.Quantity,
        //                //    TotalPrice = oi.TotalPrice
        //                //}).ToList();


        //            }
        //        }


        //    }
        //}

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
