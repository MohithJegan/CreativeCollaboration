using Microsoft.EntityFrameworkCore;
using CreativeCollaboration.Data;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models;

namespace Restaurant.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly ApplicationDbContext _context;

        public MenuItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItemDto>> ListMenuItems()
        {
            return await _context.MenuItems
                .Select(m => new MenuItemDto
                {
                    MenuItemId = m.MenuItemId,
                    MName = m.MName,
                    Price = m.Price,
                    Description = m.Description
                })
                .ToListAsync();
        }

        public async Task<MenuItemDto?> FindMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null) return null;

            return new MenuItemDto
            {
                MenuItemId = menuItem.MenuItemId,
                MName = menuItem.MName,
                Price = menuItem.Price,
                Description = menuItem.Description
            };
        }

        public async Task<ServiceResponse> AddMenuItem(AUMenuItemDto menuItemDto)
        {
            ServiceResponse serviceResponse = new();

            MenuItem menuItem = new()
            {
                MName = menuItemDto.MName,
                Price = menuItemDto.Price,
                Description = menuItemDto.Description
            };

            try
            {
                _context.MenuItems.Add(menuItem);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
                serviceResponse.CreatedId = menuItem.MenuItemId;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error adding menu item: " + ex.Message);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse> UpdateMenuItem(int id, AUMenuItemDto menuItemDto)
        {
            ServiceResponse serviceResponse = new();

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Menu item not found.");
                return serviceResponse;
            }

            menuItem.MName = menuItemDto.MName;
            menuItem.Price = menuItemDto.Price;
            menuItem.Description = menuItemDto.Description;

            try
            {
                _context.Entry(menuItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error updating menu item: " + ex.Message);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse> DeleteMenuItem(int id)
        {
            ServiceResponse serviceResponse = new();

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Menu item not found.");
                return serviceResponse;
            }

            try
            {
                _context.MenuItems.Remove(menuItem);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error deleting menu item: " + ex.Message);
            }

            return serviceResponse;
        }
    }
}
