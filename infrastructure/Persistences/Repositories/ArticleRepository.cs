using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ArticleRepository : GenericRepository<Article>, IArticleRepository
    {
        public ArticleRepository(MuseumsDbContext context): base(context) { }

        /// <summary>
        /// Returns the Articles attending to the Given Filters
        /// </summary>
        /// <param name="filters">Filters to Apply</param>
        /// <returns>A List of Articles</returns>
        public async Task<BaseEntityResponse<Article>> ListArticles(BaseFiltersRequest filters)
        {
            var response = new BaseEntityResponse<Article>();

            var articles = GetEntityQuery(x => x.DeletedAt == null && x.IsDamaged != true);

            if (!String.IsNullOrEmpty(filters.TextFilter))
            {
                articles = articles.Where(w => w.Name!.Contains(filters.TextFilter));
            }

            if (filters.IsDamagedFilter is not null)
            {
                articles = articles.Where(w => w.IsDamaged != true);
            }

            if (!String.IsNullOrEmpty(filters.StartDate) && !String.IsNullOrEmpty(filters.EndDate))
            {
                articles = articles.Where(w => w.CreatedAt >= Convert.ToDateTime(filters.StartDate) && w.CreatedAt <= Convert.ToDateTime(filters.EndDate));
            }

            if (filters.Sort is null)
                filters.Sort = "Id";

            response.TotalRecords = await articles.CountAsync();
            response.Items = await Ordering(filters, articles).ToListAsync();

            return response;
        }

        /// <summary>
        /// Returns the Articles in the Table
        /// </summary>
        /// <returns>A List of All Articles</returns>
        public async Task<BaseEntityResponse<Article>> ListAllArticles()
        {
            var response = new BaseEntityResponse<Article>();

            var articles = GetEntityQuery(x => x.DeletedAt == null && x.IsDamaged != true);

            response.TotalRecords = await articles.CountAsync();
            response.Items = await articles.ToListAsync();

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="museumId"></param>
        /// <returns></returns>
        public async Task<BaseEntityResponse<Article>> GetArticlesByMuseum(BaseFiltersRequest filters, int museumId)
        {
            var response = new BaseEntityResponse<Article>();

            var articles = GetEntityQuery(x => x.DeletedAt == null && x.IsDamaged != true);

            if (museumId != 0)
            {
                articles = articles.Where(w => w.IdMuseum == museumId);
            }

            response.TotalRecords = await articles.CountAsync();
            response.Items = await Ordering(filters, articles).ToListAsync(); 

            return response;
        }
    }
}
