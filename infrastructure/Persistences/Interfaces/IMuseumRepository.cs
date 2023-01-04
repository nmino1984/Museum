using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IMuseumRepository : IGenericRepository<Museum>
    {
        Task<BaseEntityResponse<Museum>> ListMuseums(BaseFiltersRequest filters);

        Task<BaseEntityResponse<Article>> ListArticlesByMuseum(int museumId);

        Task<BaseEntityResponse<Museum>> ListMuseumsByTheme(int theme);
    }
}
