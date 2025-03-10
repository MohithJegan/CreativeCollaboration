using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models;
using CreativeCollaboration.Models.ViewModels;

namespace CreativeCollaboration.Controllers
{
    public class StudioPageController : Controller
    {
        private readonly IStudioService _studioService;
        private readonly IMovieService _movieService;

        // dependency injection of service interface
        public StudioPageController(IStudioService StudioService, IMovieService MovieService)
        {
            _studioService = StudioService;
            _movieService = MovieService;
        }
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: StudioPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<StudioDto?> StudioDtos = await _studioService.ListStudios();
            return View(StudioDtos);
        }

        // GET: StudioPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            StudioDto? StudioDto = await _studioService.FindStudio(id);
            IEnumerable<MovieDto> Movies = await _movieService.ListMoviesForStudio(id);


            if (StudioDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find studio"] });
            }

            else
            {
                // information which drives a studio page
                StudioDetails StudioInfo = new StudioDetails()
                {
                    Studio = StudioDto,
                    Movies = Movies
                };
                return View(StudioInfo);
            }
        }

        // GET StudioPage/New
        public ActionResult New()
        {
            return View();
        }


        // POST StudioPage/Add
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(StudioDto StudioDto)
        {
            ServiceResponse response = await _studioService.AddStudio(StudioDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "StudioPage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET StudioPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            StudioDto? StudioDto = await _studioService.FindStudio(id);
            if (StudioDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(StudioDto);
            }
        }

        //POST StudioPage/Update/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(int id, StudioDto StudioDto, IFormFile StudioPic)
        {
            
            ServiceResponse imageresponse = await _studioService.UpdateStudioImage(id, StudioPic);
            // todo: error handling on imageresponse

            ServiceResponse response = await _studioService.UpdateStudio(StudioDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "StudioPage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET StudioPage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            StudioDto? StudioDto = await _studioService.FindStudio(id);
            if (StudioDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(StudioDto);
            }
        }

        //POST StudioPage/Delete/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _studioService.DeleteStudio(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "StudioPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }
    }
}
