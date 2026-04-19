using Application.Interfaces;
using Application.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;
using Utilities.Statics;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MuseumController : ControllerBase
    {
        private readonly IMuseumApplication _museumApplication;

        public MuseumController(IMuseumApplication museumApplication)
        {
            this._museumApplication = museumApplication;
        }

        [HttpGet("All")]
        public async Task<IActionResult> ListAllMuseums()
        {
            var response = await _museumApplication.ListAllMuseums();
            return Ok(response);
        }

        [HttpGet("Select")]
        public async Task<IActionResult> ListSelectMuseums()
        {
            var response = await _museumApplication.ListSelectMuseums();
            return Ok(response);
        }

        [HttpGet("{museumId:int}")]
        public async Task<IActionResult> GetMuseumById(int museumId)
        {
            var response = await _museumApplication.GetMuseumById(museumId);
            if (!response.IsSuccess && response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpGet("ArticlesByMuseum/{museumId:int}")]
        public async Task<IActionResult> ListArticlesByMuseum(int museumId)
        {
            var response = await _museumApplication.GetArticlesByMuseum(museumId);
            if (!response.IsSuccess && response.Message == ReplyMessages.MESSAGE_QUERY_EMPTY)
                return NotFound(response);
            return Ok(response);
        }

        [HttpGet("GetMuseumsByTheme/{theme:int}")]
        public async Task<IActionResult> GetMuseumsByTheme(int theme)
        {
            if (theme < 1 || theme > 3)
                return BadRequest(new { isSuccess = false, message = "Theme must be 1 (Art), 2 (Natural Sciences) or 3 (History)" });

            var response = await _museumApplication.GetMuseumsByTheme(theme);
            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterMuseum([FromBody] MuseumRequestViewModel requestViewModel)
        {
            var response = await _museumApplication.RegisterMuseum(requestViewModel);
            if (!response.IsSuccess && response.Errors is not null)
                return BadRequest(response);
            if (!response.IsSuccess)
                return BadRequest(response);
            return CreatedAtAction(nameof(GetMuseumById), new { museumId = response.Data }, response);
        }

        [HttpPut("Edit/{museumId:int}")]
        public async Task<IActionResult> EditMuseum([FromRoute] int museumId, [FromBody] MuseumRequestViewModel requestViewModel)
        {
            var response = await _museumApplication.EditMuseum(museumId, requestViewModel);
            if (!response.IsSuccess && response.Message == ReplyMessages.MESSAGE_QUERY_EMPTY)
                return NotFound(response);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("Delete/{museumId:int}")]
        public async Task<IActionResult> DeleteMuseum([FromRoute] int museumId)
        {
            var response = await _museumApplication.DeleteMuseum(museumId);
            if (!response.IsSuccess && response.Message == ReplyMessages.MESSAGE_QUERY_EMPTY)
                return NotFound(response);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("Remove/{museumId:int}")]
        public async Task<IActionResult> RemoveMuseum([FromRoute] int museumId)
        {
            var response = await _museumApplication.RemoveMuseum(museumId);
            if (!response.IsSuccess && response.Message == ReplyMessages.MESSAGE_QUERY_EMPTY)
                return NotFound(response);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }
    }
}
