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
        Task<BaseResponse<bool>> RegisterArticle(ArticleRequestViewModel requestViewModel);
        Task<BaseResponse<bool>> EditArticle(int articleId, ArticleRequestViewModel requestViewModel);
        Task<BaseResponse<bool>> DeleteArticle(int articleIdId);
        Task<BaseResponse<bool>> RemoveArticle(int articleIdId);

    }
}
