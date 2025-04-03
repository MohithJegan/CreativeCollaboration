using Microsoft.AspNetCore.Mvc;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models.ViewModels;
using CreativeCollaboration.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CreativeCollaboration.Controllers
{
    public class MoviePageController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IStudioService _studioService;
        private readonly IActorService _actorService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // dependency injection of service interface
        public MoviePageController(IMovieService MovieService, IStudioService StudioService, IActorService ActorService, ICustomerService CustomerService, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {

            _movieService = MovieService;
            _studioService = StudioService;
            _actorService = ActorService;
            _customerService = CustomerService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        [HttpGet]
        [Authorize(Roles = "admin, customer")]
        // GET: MoviePage/List
        public async Task<IActionResult> List()
        {
            IdentityUser? User = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            bool isCustomer = await _userManager.IsInRoleAsync(User, "Customer");
            ViewData["User"] = isCustomer ? "Customer" : "Admin";
            IEnumerable<MovieDto?> MovieDtos = await _movieService.ListMovies();
            return View(MovieDtos);
        }

        // GET: MoviePage/Details/{id}
        [HttpGet]
        [Authorize(Roles = "admin, customer")]
        public async Task<IActionResult> Details(int id)
        {
            MovieDto? MovieDto = await _movieService.FindMovie(id);
            IEnumerable<ActorDto> AssociatedActors = await _actorService.ListActorsForMovie(id);
            IEnumerable<ActorDto> Actors = await _actorService.ListActors();
            IEnumerable<StudioDto> Studio = await _studioService.ListStudioForMovie(id);
            IEnumerable<CustomerDto> AssociatedCustomers = await _customerService.ListCustomersForMovie(id);
            IEnumerable<CustomerDto> Customers = await _customerService.ListCustomers();
            IdentityUser? User = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
      
            if (MovieDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Movie"] });
            }
            else
            {
                bool isCustomer = await _userManager.IsInRoleAsync(User, "Customer");
                ViewData["User"] = isCustomer ? "Customer" : "Admin";
                // information which drives a Movie page
                MovieDetails MovieInfo = new MovieDetails()
                {
                    Movie = MovieDto,
                    MovieActors = AssociatedActors,
                    AllActors = Actors,
                    Studio = Studio,
                    MovieCustomers = AssociatedCustomers,
                    AllCustomers = Customers,
                };
                return View(MovieInfo);
            }
        }

        // GET MoviePage/New
        public async Task<IActionResult> New()
        {
            IEnumerable<StudioDto> Studios = await _studioService.ListStudios();
            return View(Studios);
        }


        // POST MoviePage/Add
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Add(MovieDto MovieDto)
        {
            ServiceResponse response = await _movieService.AddMovie(MovieDto);
           

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "MoviePage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET MoviePage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            MovieDto? MovieDto = await _movieService.FindMovie(id);
            //
            IEnumerable<StudioDto> Studios = await _studioService.ListStudios();
            if (MovieDto == null)
            {
                return View("Error");
            }
            else
            {
                MovieStudioDetails MovieInfo = new MovieStudioDetails()
                {
                    Movie = MovieDto,
                    Studios = Studios
                };
                return View(MovieInfo);
            }
        }

        //POST MoviePage/Update/{id}
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, MovieDto MovieDto)
        {
            ServiceResponse response = await _movieService.UpdateMovie(MovieDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "MoviePage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET MoviePage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            // Views/MoviePage/ConfirmDelete.cshtml
            // find information about this movie
            MovieDto? MovieDto = await _movieService.FindMovie(id);
            return View(MovieDto);
        }

        //POST MoviePage/Delete/{id}
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _movieService.DeleteMovie(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "MoviePage");
            }
            else
            {
                return RedirectToAction("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //POST MoviePage/LinkToActor
        //DATA: movieId={movieId}&actorId={actorId}
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> LinkToActor([FromForm] int movieId, [FromForm] int actorId)
        {
            await _movieService.LinkMovieToActor(movieId, actorId);

            return RedirectToAction("Details", new { id = movieId });
        }

        //POST MoviePage/UnlinkFromActor
        //DATA: movieId={movieId}&actorId={actorId}
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UnlinkFromActor([FromForm] int movieId, [FromForm] int actorId)
        {
            await _movieService.UnlinkMovieFromActor(movieId, actorId);

            return RedirectToAction("Details", new { id = movieId });
        }

        //POST MoviePage/LinkToCustomer
        //DATA: movieId={movieId}&customerId={actorId}
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> LinkToCustomer([FromForm] int movieId, [FromForm] int customerId)
        {
            await _movieService.LinkMovieToCustomer(movieId, customerId);

            return RedirectToAction("Details", new { id = movieId });
        }

        //POST MoviePage/UnlinkFromCustomer
        //DATA: movieId={movieId}&customerId={actorId}
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UnlinkFromCustomer([FromForm] int movieId, [FromForm] int customerId)
        {
            await _movieService.UnlinkMovieFromCustomer(movieId, customerId);

            return RedirectToAction("Details", new { id = movieId });
        }

    }
}
