using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models;


namespace CreativeCollaboration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService MovieService)
        {
            _movieService = MovieService;
        }


        /// curl -X "GET" https://localhost:7129/api/Movies/List

        /// <summary>
        /// Returns a list of Movies
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{MovieDto},{MovieDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Movies/List -> [{MovieDto},{MovieDto},..]
        /// </example>

        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<MovieDto>>> ListMovies()
        {
            // empty list of data transfer object MovieDto
            IEnumerable<MovieDto> MovieDtos = await _movieService.ListMovies();
            
            // return 200 OK with MovieDtos
            return Ok(MovieDtos);
        }

        /// curl -X "GET" https://localhost:7129/api/Movies/Find/2

        /// <summary>
        /// Returns a single Movie specified by its {id}
        /// </summary>
        /// <param name="id">The movie id</param>
        /// <returns>
        /// 200 OK
        /// {MovieDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Movies/Find/2 -> {MovieDto}
        /// </example>

        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<MovieDto>> FindMovie(int id)
        {

            var Movie = await _movieService.FindMovie(id);

            // if the movie is not found, return 404 Not Found
            if (Movie == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(Movie);
            }
        }

        /// curl -X "PUT" -H "Content-Type:application/json" -d @movie.json https://localhost:7129/api/Movies/Update/9

        /// <summary>
        /// Updates a Movie
        /// </summary>
        /// <param name="id">The ID of the movie to update</param>
        /// <param name="MovieDto">The required information to update the movie (MovieName, MovieTitle, MovieDescription)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Movies/Update/5
        /// Request Headers: Content-Type: application/json
        /// Request Body: {MovieDto}
        /// ->
        /// Response Code: 204 No Content
        /// </example>

        [HttpPut(template: "Update/{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateMovie(int id, MovieDto MovieDto)
        {
            // {id} in URL must match MovieId in POST Body
            if (id != MovieDto.MovieID)
            {
                //400 Bad Request
                return BadRequest();
            }

            ServiceResponse response = await _movieService.UpdateMovie(MovieDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            //Status = Updated
            return NoContent();

        }

        /// curl -X "POST" -H "Content-Type: application/json" -d @movie.json "https://localhost:7129/api/Movies/Add"

        /// <summary>
        /// Adds a Movie
        /// </summary>
        /// <param name="MovieDto">The required information to add the movie (MovieName, MovieTitle, MovieDescription)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Movies/Find/{MovieId}
        /// {MovieDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/Movies/Add
        /// Request Headers: Content-Type: application/json
        /// Request Body: {MovieDto}
        /// ->
        /// Response Code: 201 Created
        /// Response Headers: Location: api/Movies/Find/{MovieId}
        /// </example>

        [HttpPost(template: "Add")]
        [Authorize]
        public async Task<ActionResult<Studio>> AddMovie(MovieDto MovieDto)
        {
            ServiceResponse response = await _movieService.AddMovie(MovieDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            // returns 201 Created with Location
            return Created($"api/Movie/Find/{response.CreatedId}", MovieDto);
        }


        /// curl -X "DELETE" https://localhost:7129/api/Movies/Delete/9

        /// <summary>
        /// Deletes the Movie
        /// </summary>
        /// <param name="id">The id of the movie to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Movies/Delete/9
        /// ->
        /// Response Code: 204 No Content
        /// </example>


        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteMovie(int id)
        {
            ServiceResponse response = await _movieService.DeleteMovie(id);

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

        /// curl -X "GET" https://localhost:7129/api/Movies/ListMoviesForActor/1

        /// <summary>
        /// Returns a list of movies for a specific actor by its {id}
        /// </summary>
        /// <param name="id">The ID of the actor</param>
        /// <returns>
        /// 200 OK
        /// [{MovieDto},{MovieDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Movies/ListMoviesForActor/1 -> [{MovieDto},{MovieDto},..]
        /// </example>

        [HttpGet(template: "ListMoviesForActor/{id}")]
        public async Task<IActionResult> ListMoviesForActor(int id)
        {
            // empty list of data transfer object MovieDto
            IEnumerable<MovieDto> MovieDtos = await _movieService.ListMoviesForActor(id);
            // return 200 OK with MovieDtos
            return Ok(MovieDtos);
        }

        /// curl -X "GET" https://localhost:7129/api/Movies/ListMoviesForStudio/3

        /// <summary>
        /// Returns a list of movies for a specific studio by its {id}
        /// </summary>
        /// <param name="id">The ID of the studio</param>
        /// <returns>
        /// 200 OK
        /// [{MovieDto},{MovieDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Movies/ListMoviesForStudio/3 -> [{MovieDto},{MovieDto},..]
        /// </example>

        [HttpGet(template: "ListMoviesForStudio/{id}")]
        public async Task<IActionResult> ListMoviesForStudio(int id)
        {
            // empty list of data transfer object MovieDto
            IEnumerable<MovieDto> MovieDtos = await _movieService.ListMoviesForStudio(id);
            // return 200 OK with MovieDtos
            return Ok(MovieDtos);
        }


        /// curl -X "GET" https://localhost:7129/api/Movies/ListMoviesForActor/1

        /// <summary>
        /// Returns a list of movies for a specific actor by its {id}
        /// </summary>
        /// <param name="id">The ID of the actor</param>
        /// <returns>
        /// 200 OK
        /// [{MovieDto},{MovieDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Movies/ListMoviesForActor/1 -> [{MovieDto},{MovieDto},..]
        /// </example>

        [HttpGet(template: "ListMoviesForCustomer/{id}")]
        public async Task<IActionResult> ListMoviesForCustomer(int id)
        {
            // empty list of data transfer object MovieDto
            IEnumerable<MovieDto> MovieDtos = await _movieService.ListMoviesForCustomer(id);
            // return 200 OK with MovieDtos
            return Ok(MovieDtos);
        }



        /// curl -X "POST" "https://localhost:7129/api/Movies/Link?movieId=2&actorId=3"

        /// <summary>
        /// Links a movie from an actor
        /// </summary>
        /// <param name="movieId">The id of the movie</param>
        /// <param name="actorId">The id of the actor</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// Post: api/Movies/Link?movieId=2&actorId=3
        /// ->
        /// Response Code: 204 No Content
        /// </example>

        [HttpPost("Link")]
        [Authorize]
        public async Task<ActionResult> Link(int movieId, int actorId)
        {
            ServiceResponse response = await _movieService.LinkMovieToActor(movieId, actorId);

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

        /// curl -X "DELETE" "https://localhost:7129/api/Movies/Unlink?movieId=2&actorId=3"

        /// <summary>
        /// Unlinks a movie from an actor
        /// </summary>
        /// <param name="movieId">The id of the movie</param>
        /// <param name="actorId">The id of the actor</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// Delete: api/Movies/Unlink?movieId=2&actorId=3
        /// ->
        /// Response Code: 204 No Content
        /// </example>

        [HttpDelete("Unlink")]
        [Authorize]
        public async Task<ActionResult> Unlink(int movieId, int actorId)
        {
            ServiceResponse response = await _movieService.UnlinkMovieFromActor(movieId, actorId);

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




        /// curl -X "POST" "https://localhost:7129/api/Movies/Link?movieId=2&actorId=3"

        /// <summary>
        /// Links a movie from an actor
        /// </summary>
        /// <param name="movieId">The id of the movie</param>
        /// <param name="customerId">The id of the customer</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// Post: api/Movies/Link?movieId=2&actorId=3
        /// ->
        /// Response Code: 204 No Content
        /// </example>

        [HttpPost("LinkMovie")]
        [Authorize]
        public async Task<ActionResult> LinkMovie(int movieId, int customerId)
        {
            ServiceResponse response = await _movieService.LinkMovieToCustomer(movieId, customerId);

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

        /// curl -X "DELETE" "https://localhost:7129/api/Movies/Unlink?movieId=2&actorId=3"

        /// <summary>
        /// Unlinks a movie from an actor
        /// </summary>
        /// <param name="movieId">The id of the movie</param>
        /// <param name="customerId">The id of the customer</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// Delete: api/Movies/Unlink?movieId=2&actorId=3
        /// ->
        /// Response Code: 204 No Content
        /// </example>

        [HttpDelete("UnlinkMovie")]
        [Authorize]
        public async Task<ActionResult> UnlinkMovie(int movieId, int customerId)
        {
            ServiceResponse response = await _movieService.UnlinkMovieFromCustomer(movieId, customerId);

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



    }
}
