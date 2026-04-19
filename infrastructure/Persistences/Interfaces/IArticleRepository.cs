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

        /// <summary>
        /// Sets the IsDamaged flag to <c>true</c> for the specified article without altering any other field.
        /// </summary>
        /// <param name="articleId">Primary key of the article to mark as damaged.</param>
        /// <returns><c>true</c> if at least one row was affected; otherwise <c>false</c>.</returns>
        Task<bool> MarkAsDamagedAsync(int articleId);
    }
}
