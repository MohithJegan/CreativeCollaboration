using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models;
using CreativeCollaboration.Models.ViewModels;


namespace CreativeCollaboration.Controllers
{
    public class ActorPageController : Controller
    {

        private readonly IActorService _actorService;
        private readonly IMovieService _movieService;

        // dependency injection of service interface
        public ActorPageController(IActorService ActorService, IMovieService MovieService)
        {
            _actorService = ActorService;
            _movieService = MovieService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: ActorPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<ActorDto?> ActorDtos = await _actorService.ListActors();
            return View(ActorDtos);
        }

        // GET: ActorPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ActorDto? ActorDto = await _actorService.FindActor(id);
            IEnumerable<MovieDto> AssociatedMovies = await _movieService.ListMoviesForActor(id);
            IEnumerable<MovieDto> Movies = await _movieService.ListMovies();

            if (ActorDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find category"] });
            }
            else
            {
                // information which drives a actor page
                ActorDetails ActorInfo = new ActorDetails()
                {
                    Actor = ActorDto,
                    ActorMovies = AssociatedMovies,
                    AllMovies = Movies
                };
                return View(ActorInfo);
            }
        }

        // GET ActorPage/New
        public ActionResult New()
        {
            return View();
        }


        // POST ActorPage/Add
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(ActorDto ActorDto)
        {
            ServiceResponse response = await _actorService.AddActor(ActorDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "ActorPage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET ActorPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ActorDto? ActorDto = await _actorService.FindActor(id);
            if (ActorDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(ActorDto);
            }
        }

        //POST ActorPage/Update/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(int id, ActorDto ActorDto)
        {
            ServiceResponse response = await _actorService.UpdateActor(ActorDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "ActorPage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET ActorPage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            ActorDto? ActorDto = await _actorService.FindActor(id);
            if (ActorDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(ActorDto);
            }
        }

        //POST ActorPage/Delete/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _actorService.DeleteActor(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "ActorPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //POST ActorPage/LinkToMovie
        //DATA: actorId={actorId}&movieId={movieId}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LinkToMovie([FromForm] int actorId, [FromForm] int movieId)
        {
            await _actorService.LinkActorToMovie(actorId, movieId);

            return RedirectToAction("Details", new { id = actorId });
        }

        //POST ActorPage/UnlinkFromMovie
        //DATA: actorId={actorId}&movieId={movieId}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnlinkFromMovie([FromForm] int actorId, [FromForm] int movieId)
        {
            await _actorService.UnlinkActorFromMovie(actorId, movieId);

            return RedirectToAction("Details", new { id = actorId });
        }
    }
}
