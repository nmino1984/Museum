using Application.Interfaces;
using Application.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;
using Utilities.Statics;

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
            if (!response.IsSuccess && response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterArticle([FromBody] ArticleRequestViewModel requestViewModel)
        {
            var response = await _articleApplication.RegisterArticle(requestViewModel);
            if (!response.IsSuccess && response.Errors is not null)
                return BadRequest(response);
            if (!response.IsSuccess)
                return BadRequest(response);
            return CreatedAtAction(nameof(GetArticleById), new { articleId = response.Data }, response);
        }

        [HttpPut("Edit/{articleId:int}")]
        public async Task<IActionResult> EditArticle([FromRoute] int articleId, [FromBody] ArticleRequestViewModel requestViewModel)
        {
            var response = await _articleApplication.EditArticle(articleId, requestViewModel);
            if (!response.IsSuccess && response.Message == ReplyMessages.MESSAGE_QUERY_EMPTY)
                return NotFound(response);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Permanently deletes an Article from the database. This action cannot be undone.
        /// </summary>
        /// <param name="articleId">Identifier of the Article</param>
        [HttpDelete("Delete/{articleId:int}")]
        public async Task<IActionResult> DeleteArticle([FromRoute] int articleId)
        {
            var response = await _articleApplication.DeleteArticle(articleId);
            if (!response.IsSuccess && response.Message == ReplyMessages.MESSAGE_QUERY_EMPTY)
                return NotFound(response);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Soft-deletes an article by stamping its <c>DeletedAt</c> field.
        /// The record is retained in the database and can be recovered if needed.
        /// </summary>
        /// <param name="articleId">Identifier of the article to soft-delete.</param>
        [HttpPut("Remove/{articleId:int}")]
        public async Task<IActionResult> RemoveArticle([FromRoute] int articleId)
        {
            var response = await _articleApplication.RemoveArticle(articleId);
            if (!response.IsSuccess && response.Message == ReplyMessages.MESSAGE_QUERY_EMPTY)
                return NotFound(response);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Marks a single article as damaged by setting its <c>IsDamaged</c> flag to <c>true</c>.
        /// Only <c>IsDamaged</c> and <c>UpdatedAt</c> are written to the database.
        /// </summary>
        /// <param name="articleId">Identifier of the article to mark as damaged.</param>
        [HttpPut("MarkDamaged/{articleId:int}")]
        public async Task<IActionResult> MarkArticleAsDamaged([FromRoute] int articleId)
        {
            var response = await _articleApplication.MarkArticleAsDamaged(articleId);
            if (!response.IsSuccess && response.Message == ReplyMessages.MESSAGE_QUERY_EMPTY)
                return NotFound(response);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Creates multiple articles in a single request.
        /// All items are validated before any row is written; if one fails, the entire batch is rejected.
        /// </summary>
        /// <param name="requestViewModels">Array of article objects to create.</param>
        [HttpPost("BulkRegister")]
        public async Task<IActionResult> BulkRegisterArticles([FromBody] IEnumerable<ArticleRequestViewModel> requestViewModels)
        {
            var response = await _articleApplication.BulkRegisterArticles(requestViewModels);
            if (!response.IsSuccess && response.Errors is not null)
                return BadRequest(response);
            if (!response.IsSuccess)
                return BadRequest(response);
            return CreatedAtAction(nameof(ListAllArticles), response);
        }

        /// <summary>
        /// Moves an article from its current museum to a different one by updating the
        /// <c>IdMuseum</c> foreign key. The destination museum must exist and must not be soft-deleted.
        /// No other field is modified by this operation.
        /// </summary>
        /// <param name="articleId">Identifier of the article to relocate.</param>
        /// <param name="requestViewModel">Body containing the destination museum identifier.</param>
        [HttpPut("Relocate/{articleId:int}")]
        public async Task<IActionResult> RelocateArticle([FromRoute] int articleId, [FromBody] RelocateArticleRequestViewModel requestViewModel)
        {
            var response = await _articleApplication.RelocateArticle(articleId, requestViewModel);
            if (!response.IsSuccess && response.Message == ReplyMessages.MESSAGE_QUERY_EMPTY)
                return NotFound(response);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }
    }
}
