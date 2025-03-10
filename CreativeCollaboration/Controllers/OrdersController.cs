using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CreativeCollaboration.Data;
using CreativeCollaboration.Models;

namespace CreativeCollaboration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all orders along with customer names and total order prices.
        /// </summary>
        /// <returns>A list of orders with customer details and total price.</returns>
        /// <response code="200">Returns the list of orders.</response>
        /// <example>
        /// api/Orders/List -> [{OrderId: 1, OrderDate: "2025-01-01", CustomerName: "Isha", TotalOrderPrice: 30},{...},{...}]
        /// </example>
        [HttpGet(template:"List")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    CustomerName = o.Customer != null ? o.Customer.Name : "Unknown",
                    TotalOrderPrice = o.OrderItems.Sum(oi => oi.TotalPrice)
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific order by ID.
        /// </summary>
        /// <param name="id">The ID of the order to retrieve.</param>
        /// <returns>The order details including customer name and total price.</returns>
        /// <response code="200">Returns the order.</response>
        /// <response code="404">If the order is not found.</response>
        /// <example>
        /// api/Orders/Find/1 -> {OrderId: 1, OrderDate: "2025-01-01", CustomerName: "Isha", TotalOrderPrice: 30}
        /// </example>
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Where(o => o.OrderId == id)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    CustomerName = o.Customer != null ? o.Customer.Name : "Unknown",
                    TotalOrderPrice = o.OrderItems.Sum(oi => oi.TotalPrice)
                })
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="id">The ID of the order to update.</param>
        /// <param name="updateOrderDto">The updated order details.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Order updated successfully.</response>
        /// <response code="400">If the order ID does not match the request body.</response>
        /// <response code="404">If the order is not found.</response>
        /// <example>
        /// api/Orders/Update/{id} -> updates the Order of {id} OrderId
        /// </example>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, AUOrderDto updateOrderDto)
        {
            if (id != updateOrderDto.OrderId)
            {
                return BadRequest("Order ID mismatch.");
            }

            var order = await _context.Orders.Include(o => o.Customer).FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            // Update necessary fields
            order.OrderDate = updateOrderDto.OrderDate;

            // Update the CustomerId directly, without modifying the navigation property
            if (order.CustomerId != updateOrderDto.CustomerId)
            {
                var customer = await _context.Customers.FindAsync(updateOrderDto.CustomerId);
                if (customer == null)
                {
                    return BadRequest("Invalid CustomerId.");
                }
                order.CustomerId = updateOrderDto.CustomerId;
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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
        /// Add an existing order.
        /// </summary>
        /// <param name="id">The ID of the order to add.</param>
        /// <param name="addOrderDto">The order details we want to add.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Order updated successfully.</response>
        /// <response code="400">If the order ID does not match the request body.</response>
        /// <response code="404">If the order is not found.</response>
        /// <example>
        /// api/Orders/Add -> Add the Order of {id} OrderId
        /// </example>
        [HttpPost("Add")]
        public async Task<ActionResult<Order>> AddOrder(AUOrderDto addOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the provided CustomerId exists in the Customers table
            var customer = await _context.Customers.FindAsync(addOrderDto.CustomerId);
            if (customer == null)
            {
                return BadRequest("Invalid CustomerId: Customer does not exist.");
            }

            Order order = new Order()
            {
                OrderDate = addOrderDto.OrderDate,
                CustomerId = addOrderDto.CustomerId,
                Customer = customer 
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            AUOrderDto orderDto = new AUOrderDto()
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId
            };

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, orderDto);
        }

        /// <summary>
        /// Deletes an existing order.
        /// </summary>
        /// <param name="id">The ID of the order to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Order deleted successfully.</response>
        /// <response code="404">If the order is not found.</response>
        /// <example>
        /// api/Orders/Delete/{id} -> Deletes the Order of {id} OrderId
        /// </example>
        [HttpDelete(template: "Delete/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        /// <summary>
        /// Links a customer to an order by updating the Order's CustomerId.
        /// </summary>
        /// <param name="orderId">The ID of the order to link.</param>
        /// <param name="customerId">The ID of the customer to assign.</param>
        /// <returns>200 OK if successful, 404 Not Found if order or customer doesn't exist.</returns>
        /// <example>
        /// PUT: api/Orders/LinkCustomer/5/2 -> Assigns Customer ID 2 to Order ID 5
        /// </example>
        [HttpPut("LinkCustomer/{orderId}/{customerId}")]
        public async Task<IActionResult> LinkCustomerToOrder(int orderId, int customerId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return NotFound($"Customer with ID {customerId} not found.");
            }

            order.CustomerId = customerId;
            await _context.SaveChangesAsync();

            return Ok($"Order {orderId} is now linked to Customer {customerId}.");
        }



        /// <summary>
        /// Unlinks a customer from an order by setting CustomerId to NULL.
        /// </summary>
        /// <param name="orderId">The ID of the order to unlink.</param>
        /// <returns>200 OK if successful, 404 Not Found if order doesn't exist.</returns>
        /// <example>
        /// PUT: api/Orders/UnlinkCustomer/5 ->  Unlinks the customer from order 5
        /// </example>
        [HttpPut("UnlinkCustomer/{orderId}/{customerId}")]
        public async Task<IActionResult> UnlinkCustomerFromOrder(int orderId, int customerId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            // Check if the CustomerId matches the one linked to the order
            if (order.CustomerId == customerId)
            {
                order.CustomerId = null;
                await _context.SaveChangesAsync();
                return Ok($"Customer with ID {customerId} unlinked from Order {orderId}.");
            }

            return BadRequest("CustomerId does not match the current linked customer.");
        }


        /// <summary>
        /// Retrieves a list of orders for a specific customer, including order details such as order ID, order date, customer name, total price, and individual order items with their amounts.
        /// </summary>
        /// <param name="customerid">The ID of the customer whose orders are being retrieved.</param>
        /// <returns>A list of orders for the specified customer, or 404 Not Found if no orders exist for the given customer ID.</returns>
        /// <example>
        /// GET: api/Orders/ListOrdersForCustomer/5 -> Retrieves all orders for customer with ID 5
        /// </example>
        [HttpGet("ListOrdersForCustomer/{customerid}")]
        public async Task<ActionResult<IEnumerable<OrderWithItemsDto>>> ListOrdersForCustomer(int customerid)
        {
            var orders = await _context.Orders
                .Where(o => o.CustomerId == customerid)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Select(o => new OrderWithItemsDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    CustomerName = o.Customer != null ? o.Customer.Name : "Guest",
                    TotalOrderPrice = o.OrderItems.Sum(oi => oi.Quantity * oi.UnitOrderItemPrice),
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        OrderItemId = oi.OrderItemId,
                        MenuItemName = oi.MenuItem != null ? oi.MenuItem.MName : "Unknown Item",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitOrderItemPrice,
                        TotalPrice = oi.TotalPrice
                    }).ToList()
                })
                .ToListAsync();

            if (!orders.Any())
            {
                return NotFound($"No orders found for Customer ID {customerid}");
            }

            return Ok(orders);
        }



        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }

}
