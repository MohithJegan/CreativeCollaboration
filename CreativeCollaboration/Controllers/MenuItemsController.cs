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
    public class MenuItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MenuItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all menu items.
        /// </summary>
        /// <returns>A list of menu items.</returns>
        /// <response code="200">Returns the list of menu items.</response>
        /// <example>
        /// api/MenuItems/List -> [ {MenuItemId:1, MName: "Idli", Price: 15},{....},{....}]
        /// </example>
        [HttpGet(template:"List")]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenuItems()
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

        /// <summary>
        /// Retrieves a specific menu item by ID.
        /// </summary>
        /// <param name="id">The ID of the menu item to retrieve.</param>
        /// <returns>The menu item details.</returns>
        /// <response code="200">Returns the requested menu item.</response>
        /// <response code="404">If the menu item is not found.</response>
        /// <example>
        /// api/MenuItems/Find/1 -> {MenuItemId:1, MName: "Idli", Price: 15}
        /// </example>
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<MenuItemDto>> GetMenuItem(int id)
        {
            var menuItem = await _context.MenuItems
                .Where(m => m.MenuItemId == id)
                .Select(m => new MenuItemDto
                {
                    MenuItemId = m.MenuItemId,
                    MName = m.MName,
                    Price = m.Price
                })
                .FirstOrDefaultAsync();

            if (menuItem == null)
            {
                return NotFound();
            }

            return menuItem;
        }

        /// <summary>
        /// Updates an existing menu item.
        /// </summary>
        /// <param name="id">The ID of the menu item to update.</param>
        /// <param name="menuItemDto">The updated menu item details.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Menu item updated successfully.</response>
        /// <response code="400">If the menu item ID does not match the request body.</response>
        /// <response code="404">If the menu item is not found.</response>
        /// <example>
        /// api/MenuItems/Update/{id} -> Updates a Menu Item of that {id}
        /// </example>
        [HttpPut(template: "Update/{id}")]
        public async Task<IActionResult> PutMenuItem(int id, AUMenuItemDto menuItemDto)
        {
            if (id != menuItemDto.MenuItemId)
            {
                return BadRequest();
            }

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            // Update the menu item properties
            menuItem.MName = menuItemDto.MName;
            menuItem.Price = menuItemDto.Price;
            menuItemDto.Description = menuItemDto.Description;

            _context.Entry(menuItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuItemExists(id))
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
        /// Adds a new menu item.
        /// </summary>
        /// <param name="menuItemDto">The details of the menu item to add.</param>
        /// <returns>The newly created menu item.</returns>
        /// <response code="201">Returns the newly created menu item.</response>
        /// <example>
        /// api/MenuItems/Add -> Adds a MenuItem 
        /// </example>
        [HttpPost(template: "Add")]
        public async Task<ActionResult<AUMenuItemDto>> PostMenuItem(AUMenuItemDto menuItemDto)
        {
            var menuItem = new MenuItem
            {
                MName = menuItemDto.MName,
                Price = menuItemDto.Price,
                Description = menuItemDto.Description
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.MenuItemId }, menuItemDto);
        }

        /// <summary>
        /// Deletes an existing menu item.
        /// </summary>
        /// <param name="id">The ID of the menu item to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Menu item deleted successfully.</response>
        /// <response code="404">If the menu item is not found.</response>
        /// <example>
        /// api/MenuItems/Delete/{id} -> Deleted the MenuItem of id {id}
        /// </example>
        [HttpDelete(template: "Delete/{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.MenuItemId == id);
        }
    }
}
