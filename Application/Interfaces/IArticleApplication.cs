using Application.Commons.Bases;
using Application.ViewModels.Request;
using Application.ViewModels.Response;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IArticleApplication
    {
        Task<BaseResponse<BaseEntityResponse<ArticleResponseViewModel>>> ListArticles(BaseFiltersRequest filters);
        Task<BaseResponse<BaseEntityResponse<ArticleResponseViewModel>>> ListAllArticles();
        Task<BaseResponse<BaseEntityResponse<ArticleResponseViewModel>>> ListArticlesByMuseum(BaseFiltersRequest filters, int museumId);
        //Task<BaseResponse<IEnumerable<CategorySelectResponseViewModel>>> ListSelectCategories();
        Task<BaseResponse<ArticleResponseViewModel>> GetArticleById(int id);
        Task<BaseResponse<bool>> RegisterArticle(ArticleRequestViewModel requestViewModel);
        Task<BaseResponse<bool>> EditArticle(int articleId, ArticleRequestViewModel requestViewModel);
        Task<BaseResponse<bool>> DeleteArticle(int id);

    }
}
