using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CreativeCollaboration.Data;
using CreativeCollaboration.Models;

namespace CreativeCollaboration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// List of all Order Items
        /// </summary>
        /// <returns>
        /// A collection of OrderItem objects.
        /// 200 OK: If the order items are retrieved successfully.
        /// </returns>
        /// <example>
        /// GET: api/OrderItems/List -> [{OrderItemId : 1, MenuItemName: "Idli", UnitPrice: 15, Quantity: 2, TotalPrice: 30},{...},{...}]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetOrderItems()
        {
            var orderItems = await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    MenuItemName = oi.MenuItem.MName,
                    UnitPrice = oi.UnitOrderItemPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Quantity * oi.UnitOrderItemPrice
                })
                .ToListAsync();

            return orderItems;
        }


        /// <summary>
        /// Retrieves a specific order item by its {id}.
        /// </summary>
        /// <param name="id">The unique identifier of the order item to retrieve.</param>
        /// <returns>
        /// 200 OK: If the order item is found and returned.
        /// 404 Not Found: If the order item with the given id does not exist.
        /// </returns>
        /// <example>
        /// GET: api/OrderItems/1 -> {OrderItemId : 1, MenuItemName: "Idli", UnitPrice: 15, Quantity: 2, TotalPrice: 30}
        /// </example>
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<OrderItemDto>> GetOrderItem(int id)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .Where(oi => oi.OrderItemId == id)
                .Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    MenuItemName = oi.MenuItem.MName,
                    UnitPrice = oi.UnitOrderItemPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Quantity * oi.UnitOrderItemPrice
                })
                .FirstOrDefaultAsync();

            if (orderItem == null)
            {
                return NotFound();
            }

            return orderItem;
        }



        /// <summary>
        /// Updates an existing order item with new data.
        /// </summary>
        /// <param name="id">The unique identifier of the order item to update.</param>
        /// <param name="orderItem">The updated OrderItem object.</param>
        /// <returns>
        /// 204 No Content: If the update was successful.
        /// 400 Bad Request: If the id does not match the order item.
        /// 404 Not Found: If the order item does not exist or a concurrency issue occurs.
        /// </returns>
        /// <example>
        /// PUT: api/OrderItems/Update/{id} -> Updates the order item with id {id}.
        /// </example>
        [HttpPut(template: "Update/{id}")]
        public async Task<IActionResult> PutOrderItem(int id, AUOrderItemDto orderItemDto)
        {
            if (id != orderItemDto.OrderItemId)
            {
                return BadRequest();
            }

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            orderItem.Quantity = orderItemDto.Quantity;
            orderItem.MenuItemId = orderItemDto.MenuItemId;
            orderItem.OrderId = orderItemDto.OrderId;
            orderItem.UnitOrderItemPrice = orderItemDto.UnitOrderItemPrice;
            orderItem.TotalPrice = orderItemDto.Quantity * orderItemDto.UnitOrderItemPrice;

            _context.Entry(orderItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        /// <summary>
        /// Creates a new order item and adds it to the database.
        /// </summary>
        /// <param name="orderItem">The OrderItem object to create.</param>
        /// <returns>
        /// 201 Created: If the order item is successfully created.
        /// </returns>
        /// <example>
        /// POST: api/OrderItems/Add -> Creates a new order item.
        /// </example>
        [HttpPost(template: "Add")]
        public async Task<ActionResult<OrderItem>> PostOrderItem(AUOrderItemDto orderItemDto)
        {
            var orderItem = new OrderItem
            {
                Quantity = orderItemDto.Quantity,
                MenuItemId = orderItemDto.MenuItemId,
                OrderId = orderItemDto.OrderId,
                UnitOrderItemPrice = orderItemDto.UnitOrderItemPrice,
                TotalPrice = orderItemDto.Quantity * orderItemDto.UnitOrderItemPrice
            };

            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderItem), new { id = orderItem.OrderItemId }, orderItem);
        }



        /// <summary>
        /// Deletes a specific order item from the database.
        /// </summary>
        /// <param name="id">The unique identifier of the order item to delete.</param>
        /// <returns>
        /// 204 No Content: If the order item is successfully deleted.
        /// 404 Not Found: If the order item with the given id does not exist.
        /// </returns>
        /// <example>
        /// DELETE: api/OrderItems/Delete/{id} -> Deletes the order item with id {id}.
        /// </example>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        /// <summary>
        /// This returns the total order amount of a particular order id
        /// </summary>
        /// <param name="orderId">Id of order for which we need OrderTotal</param>
        /// <returns>total order amount of a particular order id</returns>
        /// <example>
        /// api/OrderItems/OrderTotal/2 -> 30
        /// </example>

        [HttpGet(template: "OrderTotal/{orderId}")]
        public async Task<ActionResult<decimal>> GetOrderTotal(int orderId)
        {
            var totalOrderPrice = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .SumAsync(oi => oi.Quantity * oi.MenuItem.Price);

            return Ok(totalOrderPrice);
        }



        /// <summary>
        /// This returns the list of all menu items a customer ordered
        /// </summary>
        /// <param name="customerId">The id of customer</param>
        /// <returns>list of all menu items a customer ordered</returns>
        /// <example>
        /// api/OrderItems/ListMenuItemsByCustomer/1 -> ["Dosa","Burger"]
        /// </example>
        [HttpGet("ListMenuItemsByCustomer/{customerId}")]
        public async Task<ActionResult<IEnumerable<CustomerOrderItemDto>>> GetMenuItemsByCustomer(int customerId)
        {
            var customerOrders = await _context.OrderItems
                .Where(oi => oi.Order.CustomerId == customerId)
                .Include(oi => oi.Order)
                .Include(oi => oi.MenuItem)
                .Select(oi => new CustomerOrderItemDto
                {
                    CustomerName = oi.Order.Customer.Name,
                    MenuItemName = oi.MenuItem.MName,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Quantity * oi.UnitOrderItemPrice
                })
                .ToListAsync();

            if (!customerOrders.Any())
            {
                return NotFound($"No orders found for Customer ID {customerId}");
            }

            return Ok(customerOrders);
        }


        /// <summary>
        /// Retrieves all order items for a given order ID.
        /// </summary>
        /// <param name="orderid">The ID of the order.</param>
        /// <returns>A list of order items with menu item details.</returns>
        /// <response code="200">Returns the list of order items.</response>
        /// <response code="404">If the order is not found or has no items.</response>
        /// <example>
        /// GET: api/OrderItems/1 -> [{OrderItemId: 1, MenuItemName: "Pizza", Quantity: 2, TotalPrice: 20.0}, {...}]
        /// </example>
        [HttpGet("{orderid}")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> ListOrderItemsForOrder(int orderid)
        {
            var orderItems = await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .Where(oi => oi.OrderId == orderid)
                .Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    MenuItemName = oi.MenuItem.MName,
                    UnitPrice = oi.UnitOrderItemPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.TotalPrice
                })
                .ToListAsync();

            if (orderItems == null || !orderItems.Any())
            {
                return NotFound("No order items found for the given order ID.");
            }

            return Ok(orderItems);
        }


        /// <summary>
        /// Retrieves a list of order items for a specific menu item, including details such as order item ID, quantity, unit price, and total price.
        /// </summary>
        /// <param name="menuitemid">The ID of the menu item whose order items are being retrieved.</param>
        /// <returns>A list of order items for the specified menu item, or 404 Not Found if no order items exist for the given menu item ID.</returns>
        /// <example>
        /// GET: api/OrderItems/ListOrderItemsForMenuItem/5 -> Retrieves all order items for menu item with ID 5
        /// </example>
        [HttpGet("ListOrderItemsForMenuItem/{menuitemid}")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> ListOrderItemsForMenuItem(int menuitemid)
        {
            var orderItems = await _context.OrderItems
                .Where(oi => oi.MenuItemId == menuitemid)
                .Include(oi => oi.MenuItem)
                .Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    MenuItemName = oi.MenuItem != null ? oi.MenuItem.MName : "Unknown Item",
                    UnitPrice = oi.UnitOrderItemPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.TotalPrice
                })
                .ToListAsync();

            if (!orderItems.Any())
            {
                return NotFound($"No order items found for Menu Item ID {menuitemid}");
            }

            return Ok(orderItems);
        }


        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.OrderItemId == id);
        }
    }
}
