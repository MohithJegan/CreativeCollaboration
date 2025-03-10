using Microsoft.AspNetCore.Mvc;
using CreativeCollaboration.Models;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Services;
using Microsoft.AspNetCore.Authorization;

namespace CreativeCollaboration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudioController : ControllerBase
    {
        private readonly IStudioService _studioService;

        public StudioController(IStudioService StudioService)
        {
            _studioService = StudioService;
        }



        /// curl -X "GET" https://localhost:7129/api/Studios/List 

        /// <summary>
        /// Returns a list of Studios
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{StudioDto},{StudioDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Studios/List -> [{StudioDto},{StudioDto},..]
        /// </example>

        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<StudioDto>>> ListStudios()
        {
            // empty list of data transfer object StudioDto
            IEnumerable<StudioDto> StudioDtos = await _studioService.ListStudios();

            // return 200 OK with StudioDto
            return Ok(StudioDtos);
        }


        /// curl -X "GET" https://localhost:7129/api/Studios/Find/1

        /// <summary>
        /// Returns a single Studio specified by its {id}
        /// </summary>
        /// <param name="id">The studio id</param>
        /// <returns>
        /// 200 OK
        /// {StudioDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Studios/Find/1 -> {StudioDto}
        /// </example>

        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<StudioDto>> FindStudio(int id)
        {

            var Studio = await _studioService.FindStudio(id);

            // if the studio is not found, return 404 Not Found
            if (Studio == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(Studio);
            }
        }


        /// curl -X "PUT" -H "Content-Type:application/json" -d @studio.json https://localhost:7129/api/Studios/Update/7

        /// <summary>
        /// Updates a Studio
        /// </summary>
        /// <param name="id">The ID of the studio to update</param>
        /// <param name="StudioDto">The required information to update the studio (StudioName, StudioCountry, StudioCEO)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Studios/Update/7
        /// Request Headers: Content-Type: application/json
        /// Request Body: {StudioDto}
        /// ->
        /// Response Code: 204 No Content
        /// </example>

        [HttpPut(template: "Update/{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateStudio(int id, StudioDto StudioDto)
        {
            // {id} in URL must match StudioId in POST Body
            if (id != StudioDto.StudioID)
            {
                //400 Bad Request
                return BadRequest();
            }

            ServiceResponse response = await _studioService.UpdateStudio(StudioDto);

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


        /// curl -X "POST" -H "Content-Type: application/json" -d @studio.json "https://localhost:7129/api/Studios/Add"

        /// <summary>
        /// Adds a Studio
        /// </summary>
        /// <param name="StudioDto">The required information to add the studio (StudioName, StudioCountry, StudioCEO)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Studios/Find/{StudioId}
        /// {StudioDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/Studios/Add
        /// Request Headers: Content-Type: application/json
        /// Request Body: {StudioDto}
        /// ->
        /// Response Code: 201 Created
        /// Response Headers: Location: api/Studios/Find/{StudioId}
        /// </example>

        [HttpPost(template: "Add")]
        [Authorize]
        public async Task<ActionResult<Studio>> AddStudio(StudioDto StudioDto)
        {
            ServiceResponse response = await _studioService.AddStudio(StudioDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            // returns 201 Created with Location
            return Created($"api/Studio/Find/{response.CreatedId}", StudioDto);
        }



        /// curl -X "DELETE" https://localhost:7129/api/Studios/Delete/7

        /// <summary>
        /// Deletes the Studio
        /// </summary>
        /// <param name="id">The id of the studio to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Studios/Delete/9
        /// ->
        /// Response Code: 204 No Content
        /// </example>

        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteStudio(int id)
        {
            ServiceResponse response = await _studioService.DeleteStudio(id);

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

        /// curl -X "GET" https://localhost:7129/api/Studios/ListStudioForMovie/6

        /// <summary>
        /// Returns a studio in list for a specific movie by its {id}
        /// </summary>
        /// <param name="id">The ID of the movie</param>
        /// <returns>
        /// 200 OK
        /// [{StudioDto}]
        /// </returns>
        /// <example>
        /// GET: api/Studios/ListStudioForMovie/6 -> [{StudioDto}]
        /// </example>

        [HttpGet(template: "ListStudioForMovie/{id}")]
        public async Task<IActionResult> ListStudioForMovie(int id)
        {
            // empty list of data transfer object StudioDto
            IEnumerable<StudioDto> StudioDtos = await _studioService.ListStudioForMovie(id);

            // return 200 OK with StudioDtos
            return Ok(StudioDtos);
        }

        /// <summary>
        /// Receives a studio picture and saves it to /wwwroot/images/studios/{id}{extension}
        /// </summary>
        /// <param name="id">The studio to update an image for</param>
        /// <param name="StudioPic">The picture to change to</param>
        /// <returns>
        /// 200 OK
        /// or
        /// 404 NOT FOUND
        /// or 
        /// 500 BAD REQUEST
        /// </returns>
        /// <example>
        /// PUT : api/Studios/UploadStudioPic/2
        /// HEADERS: Content-Type: Multi-part/form-data, Cookie: .AspNetCore.Identity.Application={token}
        /// FORM DATA:
        /// ------boundary
        /// Content-Disposition: form-data; name="StudioPic"; filename="mystudiopic.jpg"
        /// Content-Type: image/jpeg
        /// </example>
        /// <example>
        /// curl "https://localhost:7129/api/Studios/UploadStudioPic/1" -H "Cookie: .AspNetCore.Identity.Application={token}" -X "PUT" -F StudioPic=@mystudiopic.jpg
        /// </example>
        

        [HttpPut(template: "UploadStudioPic/{id}")]
        [Authorize]
        public async Task<IActionResult> UploadProductPic(int id, IFormFile StudioPic)
        {

            ServiceResponse response = await _studioService.UpdateStudioImage(id, StudioPic);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return Ok();

        }

    }
}
