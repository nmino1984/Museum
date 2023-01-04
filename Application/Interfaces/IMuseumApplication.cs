using Application.Commons.Bases;
using Application.ViewModels.Request;
using Application.ViewModels.Response;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IMuseumApplication
    {
        Task<BaseResponse<BaseEntityResponse<MuseumResponseViewModel>>> ListMuseums(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<MuseumSelectResponseViewModel>>> ListSelectMuseums();
        Task<BaseResponse<MuseumResponseViewModel>> GetMuseumById(int museumId);
        Task<BaseResponse<IEnumerable<ArticleRequestViewModel>>> GetArticlesByMuseum(int museumId);
        Task<BaseResponse<bool>> RegisterMuseum(MuseumRequestViewModel requestViewModel);
        Task<BaseResponse<bool>> EditMuseum(int articleId, MuseumRequestViewModel requestViewModel);
        Task<BaseResponse<bool>> DeleteMuseum(int id);

    }
}
