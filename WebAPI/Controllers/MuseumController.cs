using Application.Interfaces;
using Application.Services;
using Application.ViewModels.Request;
using Infrastructure.Commons.Bases.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(response);
        }

        [HttpGet("ArticlesByMuseum/{museumId:int}")]
        public async Task<IActionResult> ListArticlesByMuseum(int museumId)
        {
            var response = await _museumApplication.GetArticlesByMuseum(museumId);
            return Ok(response);
        }

        [HttpGet("GetMuseumsByTheme/{theme:int}")]
        public async Task<IActionResult> GetMuseumsByTheme(int theme)
        {
            var response = await _museumApplication.GetMuseumsByTheme(theme);
            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterMuseum([FromBody] MuseumRequestViewModel requestViewModel)
        {
            var response = await _museumApplication.RegisterMuseum(requestViewModel);
            return Ok(response);
        }

        [HttpPut("Edit/{museumId:int}")]
        public async Task<IActionResult> EditMuseum([FromRoute] int museumId, [FromBody] MuseumRequestViewModel requestViewModel)
        {
            var response = await _museumApplication.EditMuseum(museumId, requestViewModel);
            return Ok(response);
        }

        [HttpDelete("Delete/{museumId:int}")]
        public async Task<IActionResult> DeleteMuseum([FromRoute] int museumId)
        {
            var response = await _museumApplication.DeleteMuseum(museumId);
            return Ok(response);
        }

        [HttpPut("Remove/{museumId:int}")]
        public async Task<IActionResult> RemoveMuseum([FromRoute] int museumId)
        {
            var response = await _museumApplication.RemoveMuseum(museumId);
            return Ok(response);
        }
    }
}
