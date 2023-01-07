using Application.Interfaces;
using Application.ViewModels.Request;
using Infrastructure.Commons.Bases.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleApplication _articleApplication;

        public ArticleController(IArticleApplication articleApplication)
        {
            this._articleApplication = articleApplication;
        }

        [HttpGet("All")]
        public async Task<IActionResult> ListAllArticles()
        {
            var response = await _articleApplication.ListAllArticles();
            return Ok(response);
        }

        [HttpGet("{articleId:int}")]
        public async Task<IActionResult> GetArticleById(int articleId)
        {
            var response = await _articleApplication.GetArticleById(articleId);
            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterArticle([FromBody] ArticleRequestViewModel requestViewModel)
        {
            var response = await _articleApplication.RegisterArticle(requestViewModel);
            return Ok(response);
        }

        [HttpPut("Edit/{articleId:int}")]
        public async Task<IActionResult> EditArticle([FromRoute] int articleId, [FromBody] ArticleRequestViewModel requestViewModel)
        {
            var response = await _articleApplication.EditArticle(articleId, requestViewModel);
            return Ok(response);
        }

        /// <summary>
        /// Delete permanently an Article from Database
        /// </summary>
        /// <param name="articleId">Identifier of the Article</param>
        /// <returns>True Response if Action performs correctly</returns>
        [HttpDelete("Delete/{articleId:int}")]
        public async Task<IActionResult> DeleteArticle([FromRoute] int articleId)
        {
            var response = await _articleApplication.DeleteArticle(articleId);
            return Ok(response);
        }

        /// <summary>
        /// Removes an Article from database, updating DeletedAt field. 
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpPut("Remove/{articleId:int}")]
        public async Task<IActionResult> RemoveArticle([FromRoute] int articleId)
        {
            var response = await _articleApplication.RemoveArticle(articleId);
            return Ok(response);
        }
    }
}
