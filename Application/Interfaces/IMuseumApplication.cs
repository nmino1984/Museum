using Application.Commons.Bases;
using Application.ViewModels.Request;
using Application.ViewModels.Response;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IMuseumApplication
    {
        //Task<BaseResponse<IEnumerable<MuseumResponseViewModel>>> ListMuseums(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<MuseumResponseViewModel>>> ListAllMuseums();
        Task<BaseResponse<IEnumerable<MuseumSelectResponseViewModel>>> ListSelectMuseums();
        Task<BaseResponse<MuseumResponseViewModel>> GetMuseumById(int museumId);
        Task<BaseResponse<IEnumerable<MuseumResponseViewModel>>> GetMuseumsByTheme(int theme);
        Task<BaseResponse<IEnumerable<ArticleResponseViewModel>>> GetArticlesByMuseum(int museumId);
        Task<BaseResponse<bool>> RegisterMuseum(MuseumRequestViewModel requestViewModel);
        Task<BaseResponse<bool>> EditMuseum(int articleId, MuseumRequestViewModel requestViewModel);
        Task<BaseResponse<bool>> DeleteMuseum(int id);
        Task<BaseResponse<bool>> RemoveMuseum(int id);

    }
}
