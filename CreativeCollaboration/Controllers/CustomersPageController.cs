using Microsoft.AspNetCore.Mvc;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models.ViewModels;
using CreativeCollaboration.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CreativeCollaboration.Services;
using Microsoft.AspNetCore.Authorization;

namespace CreativeCollaboration.Controllers
{
    public class CustomerPageController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IMovieService _movieService;
        //private readonly IQuizService _quizService;
        // Dependency injection of service interfaces
        public CustomerPageController(ICustomerService CustomerService, IOrderService OrderService, IMovieService MovieService)
        {
            _customerService = CustomerService;
            _orderService = OrderService;
            _movieService = MovieService;
            //_quizService = quizService;
        }

        // Show List of Customers on Index page 
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: CustomerPage/ListCustomers
        [HttpGet("ListCustomers")]
        [Authorize(Roles = "admin, customer")]
        public async Task<IActionResult> List()
        {
            string UserRole = "";
            IEnumerable<CustomerDto?> customerDtos = await _customerService.ListCustomers();
            if (customerDtos.Count() > 1)
            {
                UserRole = "Admin";
                ViewData["User"] = UserRole;
            }
            else
            {
                UserRole = "Customer";
                ViewData["User"] = UserRole;
            }
            return View(customerDtos);
        }

        // GET: CustomerPage/CustomerDetails/{id}
        [HttpGet("CustomerDetails/{id}")]
        [Authorize(Roles = "admin, customer")]
        public async Task<IActionResult> Details(int id)
        {
            CustomerDto? customerDto = await _customerService.FindCustomer(id);
            IEnumerable<MovieDto> AssociatedMovies = await _movieService.ListMoviesForCustomer(id);
            IEnumerable<MovieDto> Movies = await _movieService.ListMovies();

            if (customerDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Customer"] });
            }
            else
            {
                var orders = await _orderService.ListOrdersByCustomerId(id);

                CustomerDetails customerInfo = new CustomerDetails()
                {
                    Customer = customerDto,
                    Order = orders.ToList(),
                    CustomerMovies = AssociatedMovies,
                    AllMovies = Movies
                    
                };

                return View(customerInfo);
            }
        }
        // GET: CustomerPage/AddCustomer
        [HttpGet("AddCustomer")]
        public IActionResult AddCustomer()
        {
            return View("AddCustomer");
        }

        // POST: CustomerPage/AddCustomer
        [HttpPost("AddCustomer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCustomer(AUCustomerDto customerDto)
        {
            if (ModelState.IsValid)
            {
                await _customerService.AddCustomer(customerDto);
                return RedirectToAction("List");
            }

            return View("AddCustomer", customerDto);
        }
        // GET: CustomerPage/EditCustomer/{id}
        [HttpGet("Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            CustomerDto? customerDto = await _customerService.FindCustomer(id);

            if (customerDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Customer not found"] });
            }

            var updateCustomerDto = new AUCustomerDto
            {
                CustomerId = customerDto.CustomerId,
                Name = customerDto.Name,
                Email = customerDto.Email,
                Phone = customerDto.Phone
            };

            return View(updateCustomerDto);
        }

        // POST: CustomerPage/EditCustomer/{id}
        [HttpPost("Edit/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AUCustomerDto updateCustomerDto)
        {
            if (id != updateCustomerDto.CustomerId)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Invalid customer ID"] });
            }

            if (ModelState.IsValid)
            {
                var serviceResponse = await _customerService.UpdateCustomer(id, updateCustomerDto);

                if (serviceResponse.Status == ServiceResponse.ServiceStatus.Error)
                {
                    return View("Error", new ErrorViewModel() { Errors = serviceResponse.Messages });
                }

                return RedirectToAction("Details", new { id });
            }

            return View(updateCustomerDto);
        }
        // ✅ GET: CustomerPage/DeleteCustomer/{id}
        [HttpGet("DeleteCustomer/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            CustomerDto? customerDto = await _customerService.FindCustomer(id);

            if (customerDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Customer not found"] });
            }

            return View("DeleteCustomer", customerDto); // Ensure this matches the view file name
        }
        // ✅ POST: CustomerPage/DeleteCustomer/{id}
        [HttpPost("DeleteCustomer/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _customerService.DeleteCustomer(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }


        //POST ActorPage/LinkToMovie
        //DATA: actorId={actorId}&movieId={movieId}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LinkToMovie([FromForm] int customerId, [FromForm] int movieId)
        {
            await _customerService.LinkCustomerToMovie(customerId, movieId);

            return RedirectToAction("Details", new { id = customerId });
        }

        //POST ActorPage/UnlinkFromMovie
        //DATA: actorId={actorId}&movieId={movieId}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnlinkFromMovie([FromForm] int customerId, [FromForm] int movieId)
        {
            await _customerService.UnlinkCustomerFromMovie(customerId, movieId);

            return RedirectToAction("Details", new { id = customerId });
        }


    }
}
