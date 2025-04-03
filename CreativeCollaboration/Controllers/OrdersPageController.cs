using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models;
using CreativeCollaboration.Models.ViewModels;
using Restaurant.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace CreativeCollaboration.Controllers
{
    public class OrdersPageController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IOrderItemService _orderItemService;
        private readonly IMenuItemService _menuItemService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrdersPageController(IOrderService orderService, ICustomerService customerService, IOrderItemService orderItemService,
    IMenuItemService menuItemService, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _customerService = customerService;
            _orderItemService = orderItemService;
            _menuItemService = menuItemService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // ✅ GET: OrdersPage/List
        [HttpGet("Orders/List")]
        public async Task<IActionResult> List()
        {
            IEnumerable<OrderDto> orders = await _orderService.ListOrders();
            IdentityUser? User = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (User != null)
            {
                bool isCustomer = await _userManager.IsInRoleAsync(User, "Customer");
                ViewData["User"] = isCustomer ? "Customer" : "Admin";
                return View(orders);
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = ["Please register to our application"] });
            }
           
        }

        // ✅ GET: OrdersPage/Details/{id}
        [HttpGet("Orders/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            IdentityUser? User = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            bool isCustomer = await _userManager.IsInRoleAsync(User, "Customer");
            ViewData["User"] = isCustomer ? "Customer" : "Admin";
            OrderDto? order = await _orderService.FindOrder(id);
            if (order == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Order not found"] });
            }

            var orderDetailsViewModel = new OrderDetailsViewModel
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                TotalOrderPrice = order.TotalOrderPrice,
                OrderItems = order.OrderItems
            };

            return View(orderDetailsViewModel);
        }


        [HttpGet("Orders/Add")]
        public async Task<IActionResult> Add()
        {
            var customers = await _orderService.ListCustomers(); // Fetch customers for dropdown
            var menuItems = await _orderService.ListMenuItems();

            ViewBag.Customers = customers.Select(c => new SelectListItem
            {
                Value = c.CustomerId.ToString(),
                Text = c.Name
            }).ToList();

            ViewBag.MenuItems = menuItems.Select(m => new SelectListItem
            {
                Value = m.MenuItemId.ToString(),
                Text = $"{m.MName} - ${m.Price}" // Show price for reference
            }).ToList();

            return View(new AUOrderDto());
        }


        [HttpPost("Orders/Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AUOrderDto orderDto)
        {
            if (ModelState.IsValid)
            {
                var response = await _orderService.AddOrder(orderDto);
                if (response.Status == ServiceResponse.ServiceStatus.Created)
                {
                    return RedirectToAction("List");
                }
                else
                {
                    return View("Error", new ErrorViewModel { Errors = response.Messages });
                }
            }
            return View(orderDto);
        }


        [HttpGet("Orders/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderService.FindOrder(id);
            if (order == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Order not found"] });
            }

            // Fetch customers for dropdown
            var customers = await _customerService.ListCustomers();
            ViewBag.Customers = customers.Select(c => new SelectListItem
            {
                Value = c.CustomerId.ToString(),
                Text = c.Name
            }).ToList();

            // Fetch menu items for dropdown
            var menuItems = await _menuItemService.ListMenuItems();
            ViewBag.MenuItems = menuItems.Select(m => new SelectListItem
            {
                Value = m.MenuItemId.ToString(),
                Text = $"{m.MName} - ${m.Price}" // Display name with price
            }).ToList();

            // ✅ Convert OrderDto to AUOrderDto
            var updateOrderDto = new AUOrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                OrderItems = order.OrderItems.Select(oi => new AUOrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    MenuItemId = oi.MenuItemId,
                    Quantity = oi.Quantity,
                    UnitOrderItemPrice = oi.UnitPrice,  // Ensuring the Unit Price is loaded
                    TotalPrice = oi.Quantity * oi.UnitPrice // Correct calculation for Total Price
                }).ToList()
            };

            return View(updateOrderDto);
        }


        [HttpPost("Orders/Edit/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AUOrderDto updateOrderDto)
        {
            if (id != updateOrderDto.OrderId)
            {
                return View("Error", new ErrorViewModel { Errors = ["Order ID mismatch"] });
            }

            if (ModelState.IsValid)
            {
                var response = await _orderService.UpdateOrder(id, updateOrderDto);
                if (response.Status == ServiceResponse.ServiceStatus.Updated)
                {
                    return RedirectToAction("Details", new { id });
                }
                else
                {
                    return View("Error", new ErrorViewModel { Errors = response.Messages });
                }
            }

            // Reload dropdowns if validation fails
            ViewBag.Customers = (await _customerService.ListCustomers()).Select(c => new SelectListItem
            {
                Value = c.CustomerId.ToString(),
                Text = c.Name
            }).ToList();

            ViewBag.MenuItems = (await _menuItemService.ListMenuItems()).Select(m => new SelectListItem
            {
                Value = m.MenuItemId.ToString(),
                Text = $"{m.MName} - ${m.Price}"
            }).ToList();

            return View(updateOrderDto);
        }


        // ✅ GET: OrdersPage/Delete/{id} (Show Delete Confirmation)
        [HttpGet("Orders/Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            OrderDto? order = await _orderService.FindOrder(id);
            if (order == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Order not found"] });
            }

            return View(order);
        }

        // ✅ POST: OrdersPage/Delete/{id} (Confirm Deletion)
        [HttpPost("Orders/Delete/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _orderService.DeleteOrder(id);
            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }

    }
}
