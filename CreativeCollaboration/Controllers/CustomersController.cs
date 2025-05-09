﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using CreativeCollaboration.Data;
using CreativeCollaboration.Models;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CreativeCollaboration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomerService _customerService;

        public CustomersController(ApplicationDbContext context, ICustomerService CustomerService)
        {
            _context = context;
            _customerService = CustomerService;
        }



        /// <summary>
        /// Gets a list of customers in the system. Administrator only.
        /// </summary>
        /// <returns>
        /// 200 Ok
        /// List of Customer including ID, Name, Last Order Date and Last Order Price.
        /// </returns>
        /// <example>
        /// GET: api/Customers/List -> [{CustomerId: 1, Name: "Himani", LastOrderDate: "2025-01-01", LastOrderPrice:30},{....},{....}]
        /// HEADERS: Cookie: .AspNetCore.Identity.Application={token}
        /// </example>
        [HttpGet(template: "List")]
        [Authorize(Roles="admin")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> ListCustomers()
        {
            List<Customer> Customers = await _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.OrderItems)
                .ToListAsync();

            List<CustomerDto> CustomerDtos = new List<CustomerDto>();

            foreach (Customer customer in Customers)
            {
                var lastOrder = customer.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .FirstOrDefault();

                CustomerDtos.Add(new CustomerDto()
                {
                    CustomerId = customer.CustomerId,
                    Name = customer.Name,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    City = customer.City,
                    State = customer.State,
                    Country = customer.Country,
                    CustomerAccountId = customer.CustomerAccountId,
                    LastOrderDate = lastOrder?.OrderDate ?? (new DateOnly(DateOnly.FromDateTime(DateTime.Now).Year, DateOnly.FromDateTime(DateTime.Now).Month, DateOnly.FromDateTime(DateTime.Now).Day)),
                    LastOrderPrice = lastOrder?.OrderItems?.Sum(oi => oi.TotalPrice) ?? 0
                });
            }

            return Ok(CustomerDtos);
        }



        /// <summary>
        /// Finds a customer in the system. Administrators can access any customer.
        /// </summary>
        /// <param name="id">Customer's id</param>
        /// <returns>
        /// 200 Ok
        /// CustomerDto : It includes Customer including ID, Name, Last Order Date and Last Order Price.
        /// or
        /// 404 Not Found when there is no Customer for that {id}
        /// </returns>
        /// <example>
        /// HEADERS: Cookie: .AspNetCore.Identity.Application={token}
        /// GET: api/Customers/Find/1 -> {CustomerId: 1, Name: "Himani", LastOrderDate: "2025-01-01", LastOrderPrice:30}
        [HttpGet(template: "Find/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<CustomerDto>> FindCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.OrderItems)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            var lastOrder = customer.Orders
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefault();

            CustomerDto customerDto = new CustomerDto()
            {
                CustomerId = customer.CustomerId,
                CustomerAccountId = customer.CustomerAccountId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                LastOrderDate = lastOrder.OrderDate,
                LastOrderPrice = lastOrder?.OrderItems?.Sum(oi => oi.TotalPrice) ?? 0
            };

            return Ok(customerDto);
        }


        /// <summary>
        /// Updates a Customer record. Administrator only.
        /// </summary>
        /// <param name="id">The ID of the Customer to update</param>
        /// <param name="updateCustomerDto">The required information to update the Customer</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>  
        /// <example>
        /// PUT: api/Customer/Update/5
        /// Request Headers: Content-Type: application/json, cookie: .AspNetCore.Identity.Application={token}
        /// Request Body: {CustomerDto}
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        [HttpPut(template: "Update/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCustomer(int id, AUCustomerDto updateCustomerDto)
        {
            if (id != updateCustomerDto.CustomerId)
            {
                return BadRequest("Customer ID mismatch.");
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Update necessary fields
            customer.Name = updateCustomerDto.Name;
            customer.Email = updateCustomerDto.Email;
            customer.Phone = updateCustomerDto.Phone;

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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
        /// Adds a Customer to the Customers table. Administrator only. 
        /// </summary>
        /// <remarks>
        /// Adds a Customer using AUCustomerDto as input, and returns the added Customer details.
        /// </remarks>
        /// <param name="addCustomerDto">The required information to add the Customer</param>
        /// <returns>
        /// 201 Created
        /// or
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// api/Customers/Add -> Adds a new Customer 
        /// Request Headers: Content-Type: application/json, cookie: .AspNetCore.Identity.Application={token}
        /// Request Body: {CustomerDto}
        /// ->
        /// Response Code: 201 Created
        /// Response Headers: Location: api/Customer/Find/{CustomerId}
        /// </example>
        [HttpPost(template: "Add")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Customer>> AddCustomer(AUCustomerDto addCustomerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer customer = new Customer()
            {
                Name = addCustomerDto.Name,
                Email = addCustomerDto.Email,
                Phone = addCustomerDto.Phone
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            AUCustomerDto customerDto = new AUCustomerDto()
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone
            };

            return CreatedAtAction(nameof(FindCustomer), new { id = customer.CustomerId }, customerDto);
        }

        /// <summary>
        /// Delete a Customer specified by it's {id}. Administrator only.
        /// </summary>
        /// <param name="id">The id of the Customer we want to delete</param>
        /// <returns>
        /// 201 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// api/Customer/Delete/{id} -> Deletes the Customer associated with {id}
        /// Headers: cookie: .AspNetCore.Identity.Application={token}
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        [HttpDelete(template: "Delete/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }


        /// curl -X "GET" https://localhost:7129/api/Customer/ListCustomersForMovie/2

        /// <summary>
        /// Returns a list of customers for a specific movie by its {id}. Administrator only.
        /// </summary>
        /// <param name="id">The ID of the movie</param>
        /// <returns>
        /// 200 OK
        /// [{CustomerDto},{CustomerDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Customer/ListActorsForMovie/1 -> [{CustomerDto},{CustomerDto},..]
        /// HEADERS: Cookie: .AspNetCore.Identity.Application={token}
        /// -> [{CustomerDto},{CustomerDto},..]
        /// </example>

        [HttpGet(template: "ListCustomersForMovie/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ListCustomersForMovie(int id)
        {
            // empty list of data transfer object MovieDto
            IEnumerable<CustomerDto> CustomerDtos = await _customerService.ListCustomersForMovie(id);

            // return 200 OK with MovieDtos
            return Ok(CustomerDtos);
        }


        /// curl -X "POST" "https://localhost:7129/api/Customers/Link?customerId=2movieId=2"

        /// <summary>
        /// Links a movie from an actor. Administrator only.
        /// </summary>
        /// <param name="customerId">The id of the customer</param>
        /// <param name="movieId">The id of the movie</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// Post: api/Customers/Link?customerId=2&actorId=3
        /// HEADERS: Cookie: .AspNetCore.Identity.Application={token}
        /// ->
        /// Response Code: 204 No Content
        /// </example>

        [HttpPost("Link")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Link(int customerId, int movieId)
        {
            ServiceResponse response = await _customerService.LinkCustomerToMovie(customerId, movieId);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();

        }

        /// curl -X "DELETE" "https://localhost:7129/api/Customers/Unlink?&customerId=2movieId=2"

        /// <summary>
        /// Unlinks a movie from an customer.  Administrator only.
        /// </summary>
        /// <param name="customerId">The id of the customer</param>
        /// <param name="movieId">The id of the movie</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// Delete: api/Customers/Unlink?customerId=2&actorId=3
        /// HEADERS: Cookie: .AspNetCore.Identity.Application={token}
        /// ->
        /// Response Code: 204 No Content
        /// </example>

        [HttpDelete("Unlink")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Unlink(int customerId, int movieId)
        {
            ServiceResponse response = await _customerService.UnlinkCustomerFromMovie(customerId, movieId);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();

        }

        /// curl -X "DELETE" "https://localhost:7129/api/Customers/Profile"

        /// <summary>
        /// Gets a single customer's profile (based on their logged in information)
        /// </summary>
        /// <returns>
        /// CustomerDto
        /// </returns>
        /// <example>
        /// GET: api/Customer/Profile
        ///  HEADERS: Cookie: .AspNetCore.Identity.Application={token}
        /// -> {CustomerDto}
        /// </example>

        [HttpGet(template: "Profile")]
        [Authorize(Roles = "admin,customer")]
        public async Task<ActionResult<CustomerDto>> Profile()
        {

            CustomerDto? Customer = await _customerService.Profile();
            // if the Customer could not be located, return 404 Not Found
            if (Customer == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(Customer);
            }
        }

    }
}