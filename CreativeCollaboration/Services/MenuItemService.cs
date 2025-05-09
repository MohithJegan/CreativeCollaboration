﻿using Microsoft.EntityFrameworkCore;
using CreativeCollaboration.Data;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models;
using Microsoft.AspNetCore.Identity;

namespace Restaurant.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MenuItemService(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<MenuItemDto>> ListMenuItems()
        {
            return await _context.MenuItems
                .Select(m => new MenuItemDto
                {
                    MenuItemId = m.MenuItemId,
                    MName = m.MName,
                    Price = m.Price,
                    Description = m.Description,
                    ImagePath = m.ImagePath
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
                Description = menuItem.Description,
                ImagePath = menuItem.ImagePath
            };
        }

        public async Task<ServiceResponse> AddMenuItem(AUMenuItemDto menuItemDto)
        {
            ServiceResponse serviceResponse = new();

            MenuItem menuItem = new()
            {
                MName = menuItemDto.MName,
                Price = menuItemDto.Price,
                Description = menuItemDto.Description,
                ImagePath = menuItemDto.ImagePath
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
            menuItem.ImagePath = menuItemDto.ImagePath;

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
