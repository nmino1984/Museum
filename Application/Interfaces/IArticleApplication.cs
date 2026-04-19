using Application.Commons.Bases;
using Application.ViewModels.Request;
using Application.ViewModels.Response;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IArticleApplication
    {
        //Task<BaseResponse<IEnumerable<ArticleResponseViewModel>>> ListArticles(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<ArticleResponseViewModel>>> ListAllArticles();
        //Task<BaseResponse<IEnumerable<ArticleResponseViewModel>>> ListArticlesByMuseum(BaseFiltersRequest filters, int museumId);
        //Task<BaseResponse<IEnumerable<CategorySelectResponseViewModel>>> ListSelectCategories();
        Task<BaseResponse<ArticleResponseViewModel>> GetArticleById(int id);
        Task<BaseResponse<int>> RegisterArticle(ArticleRequestViewModel requestViewModel);

        /// <summary>
        /// Validates and persists a collection of articles in a single database round-trip.
        /// If any item fails validation the entire operation is aborted and no row is inserted.
        /// Returns the count of inserted articles on success.
        /// </summary>
        Task<BaseResponse<int>> BulkRegisterArticles(IEnumerable<ArticleRequestViewModel> requestViewModels);

        Task<BaseResponse<bool>> EditArticle(int articleId, ArticleRequestViewModel requestViewModel);
        Task<BaseResponse<bool>> DeleteArticle(int articleIdId);
        Task<BaseResponse<bool>> RemoveArticle(int articleIdId);

        /// <summary>
        /// Sets <c>IsDamaged = true</c> on the specified article.
        /// Returns a failure response if the article does not exist or is already soft-deleted.
        /// </summary>
        Task<BaseResponse<bool>> MarkArticleAsDamaged(int articleId);

        /// <summary>
        /// Moves an article to a different museum by updating its <c>IdMuseum</c> foreign key.
        /// Validates that both the article and the destination museum exist before persisting.
        /// </summary>
        Task<BaseResponse<bool>> RelocateArticle(int articleId, RelocateArticleRequestViewModel requestViewModel);
    }
}
