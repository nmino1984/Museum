using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IArticleRepository : IGenericRepository<Article>
    {
        Task<BaseEntityResponse<Article>> ListArticles(BaseFiltersRequest filters);
        Task<BaseEntityResponse<Article>> ListAllArticles();
        Task<BaseEntityResponse<Article>> GetArticlesByMuseum(BaseFiltersRequest filters, int museumId);
    }
}
