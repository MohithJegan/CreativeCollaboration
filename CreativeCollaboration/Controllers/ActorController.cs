using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models;
using CreativeCollaboration.Services;


namespace CreativeCollaboration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly IActorService _actorService;

        public ActorController(IActorService ActorService)
        {
            _actorService = ActorService;
        }



        /// curl -X "GET" https://localhost:7129/api/Actors/List 

        /// <summary>
        /// Returns a list of Actors
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{ActorDto},{ActorDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Actors/List -> [{ActorDto},{ActorDto},..]
        /// </example>

        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<ActorDto>>> ListActors()
        {
            // empty list of data transfer object ActorDto
            IEnumerable<ActorDto> ActorDtos = await _actorService.ListActors();

            // return 200 OK with ActorDtos
            return Ok(ActorDtos);
        }


        /// curl -X "GET" https://localhost:7129/api/Actors/Find/1

        /// <summary>
        /// Returns a single Actor specified by its {id}
        /// </summary>
        /// <param name="id">The actor id</param>
        /// <returns>
        /// 200 OK
        /// {ActorDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Actors/Find/1 -> {ActorDto}
        /// </example>

        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<ActorDto>> FindActor(int id)
        {

            var Actor = await _actorService.FindActor(id);

            // if the actor is not found, return 404 Not Found
            if (Actor == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(Actor);
            }
        }


        /// curl -X "PUT" -H "Content-Type:application/json" -d @actor.json https://localhost:7129/api/Actors/Update/7

        /// <summary>
        /// Updates an Actor
        /// </summary>
        /// <param name="id">The ID of the actor to update</param>
        /// <param name="ActorDto">The required information to update the actor (ActorName, ActorDOB, ActorDebutYear)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Actors/Update/7
        /// Request Headers: Content-Type: application/json
        /// Request Body: {ActorDto}
        /// ->
        /// Response Code: 204 No Content
        /// </example>

        [HttpPut(template: "Update/{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateStudio(int id, ActorDto ActorDto)
        {
            // {id} in URL must match ActorId in POST Body
            if (id != ActorDto.ActorId)
            {
                //400 Bad Request
                return BadRequest();
            }

            ServiceResponse response = await _actorService.UpdateActor(ActorDto);

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


        /// curl -X "POST" -H "Content-Type: application/json" -d @actor.json "https://localhost:7129/api/Actors/Add"

        /// <summary>
        /// Adds an Actor
        /// </summary>
        /// <param name="ActorDto">The required information to add the actor (ActorName, ActorDOB, ActorDebutYear)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Actors/Find/{ActorId}
        /// {ActorDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/Actors/Add
        /// Request Headers: Content-Type: application/json
        /// Request Body: {ActorDto}
        /// ->
        /// Response Code: 201 Created
        /// Response Headers: Location: api/Actors/Find/{ActorId}
        /// </example>

        [HttpPost(template: "Add")]
        [Authorize]
        public async Task<ActionResult<Actor>> AddActor(ActorDto ActorDto)
        {
            ServiceResponse response = await _actorService.AddActor(ActorDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            // returns 201 Created with Location
            return Created($"api/Actor/Find/{response.CreatedId}", ActorDto);
        }

        /// curl -X "DELETE" https://localhost:7129/api/Actors/Delete/7

        /// <summary>
        /// Deletes the Actor
        /// </summary>
        /// <param name="id">The id of the actor to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Actors/Delete/7
        /// ->
        /// Response Code: 204 No Content
        /// </example>

        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteActor(int id)
        {
            ServiceResponse response = await _actorService.DeleteActor(id);

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


        /// curl -X "GET" https://localhost:7129/api/Actors/ListActorsForMovie/2

        /// <summary>
        /// Returns a list of actors for a specific movie by its {id}
        /// </summary>
        /// <param name="id">The ID of the movie</param>
        /// <returns>
        /// 200 OK
        /// [{ActorDto},{ActorDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Actors/ListActorsForMovie/1 -> [{ActorDto},{ActorDto},..]
        /// </example>

        [HttpGet(template: "ListActorsForMovie/{id}")]
        public async Task<IActionResult> ListActorsForMovie(int id)
        {
            // empty list of data transfer object MovieDto
            IEnumerable<ActorDto> ActorDtos = await _actorService.ListActorsForMovie(id);

            // return 200 OK with MovieDtos
            return Ok(ActorDtos);
        }

        /// curl -X "POST" "https://localhost:7129/api/Movies/Link?actorId=2movieId=2"

        /// <summary>
        /// Links a movie from an actor
        /// </summary>
        /// <param name="actorId">The id of the actor</param>
        /// <param name="movieId">The id of the movie</param>
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
        public async Task<ActionResult> Link(int actorId, int movieId)
        {
            ServiceResponse response = await _actorService.LinkActorToMovie(actorId, movieId);

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

        /// curl -X "DELETE" "https://localhost:7129/api/Actors/Unlink?&actorId=2movieId=2"

        /// <summary>
        /// Unlinks a movie from an actor
        /// </summary>
        /// <param name="actorId">The id of the actor</param>
        /// <param name="movieId">The id of the movie</param>
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
        public async Task<ActionResult> Unlink(int actorId, int movieId)
        {
            ServiceResponse response = await _actorService.UnlinkActorFromMovie(actorId, movieId);

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
