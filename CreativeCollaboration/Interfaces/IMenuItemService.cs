using CreativeCollaboration.Models;

namespace CreativeCollaboration.Interfaces
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItemDto>> ListMenuItems();
        Task<MenuItemDto?> FindMenuItem(int id);
        Task<ServiceResponse> AddMenuItem(AUMenuItemDto menuItemDto);
        Task<ServiceResponse> UpdateMenuItem(int id, AUMenuItemDto menuItemDto);
        Task<ServiceResponse> DeleteMenuItem(int id);
    }
}
