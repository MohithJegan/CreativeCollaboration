using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models;
using CreativeCollaboration.Models.ViewModels;
using CreativeCollaboration.Services;

namespace CreativeCollaboration.Controllers
{
    [Route("MenuItemsPage")]
    public class MenuItemsPageController : Controller
    {
        private readonly IMenuItemService _menuItemService;

        // Dependency injection of MenuItemService
        public MenuItemsPageController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        // Show list of Menu Items on Index page 
        [HttpGet("")]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: MenuItemsPage/List
        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            IEnumerable<MenuItemDto> menuItems = await _menuItemService.ListMenuItems();
            return View("List", menuItems);
        }

        // GET: MenuItemsPage/MenuItemDetails/{id}
        [HttpGet("MenuItemDetails/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            MenuItemDto? menuItem = await _menuItemService.FindMenuItem(id);
            if (menuItem == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Could not find menu item"] });
            }

            var menuItemDetails = new MenuItemDetails
            {
                MenuItem = menuItem
            };

            return View(menuItem);
        }

        // ✅ GET: MenuItemsPage/AddMenuItem
        [HttpGet("AddMenuItem")]
        public IActionResult Add()
        {
            return View();
        }

        // ✅ POST: MenuItemsPage/AddMenuItem
        [HttpPost("AddMenuItem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AUMenuItemDto addMenuItemDto)
        {
            if (ModelState.IsValid)
            {
                ServiceResponse response = await _menuItemService.AddMenuItem(addMenuItemDto);
                if (response.Status == ServiceResponse.ServiceStatus.Created)
                {
                    return RedirectToAction("List");
                }
                else
                {
                    return View("Error", new ErrorViewModel() { Errors = response.Messages });
                }
            }
            return View(addMenuItemDto);
        }

        // ✅ GET: MenuItemsPage/EditMenuItem/{id}
        [HttpGet("EditMenuItem/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            MenuItemDto? menuItem = await _menuItemService.FindMenuItem(id);
            if (menuItem == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Menu item not found"] });
            }

            var updateMenuItemDto = new AUMenuItemDto
            {
                MenuItemId = menuItem.MenuItemId,
                MName = menuItem.MName,
                Description = menuItem.Description,
                Price = menuItem.Price
            };

            return View(updateMenuItemDto);
        }

        // ✅ POST: MenuItemsPage/EditMenuItem/{id}
        [HttpPost("EditMenuItem/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AUMenuItemDto updateMenuItemDto)
        {
            if (id != updateMenuItemDto.MenuItemId)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Menu item ID mismatch"] });
            }

            if (ModelState.IsValid)
            {
                var serviceResponse = await _menuItemService.UpdateMenuItem(id, updateMenuItemDto);

                if (serviceResponse.Status == ServiceResponse.ServiceStatus.Error)
                {
                    return View("Error", new ErrorViewModel() { Errors = serviceResponse.Messages });
                }

                return RedirectToAction("Details", new { id });
            }

            return View(updateMenuItemDto);
        }

        // ✅ GET: MenuItemsPage/DeleteMenuItem/{id}
        [HttpGet("DeleteMenuItem/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            MenuItemDto? menuItem = await _menuItemService.FindMenuItem(id);
            if (menuItem == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Menu item not found"] });
            }

            return View("ConfirmDelete", menuItem);
        }

        // ✅ POST: MenuItemsPage/DeleteMenuItem/{id}
        [HttpPost("DeleteMenuItem/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _menuItemService.DeleteMenuItem(id);
            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }
    }
}
