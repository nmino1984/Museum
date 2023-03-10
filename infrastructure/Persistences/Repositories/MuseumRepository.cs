using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Infrastructure.Persistences.Repositories
{
    public class MuseumRepository : GenericRepository<Museum>, IMuseumRepository
    {
        public MuseumRepository(MuseumsDbContext context) : base(context) { }

        public async Task<BaseEntityResponse<Article>> ListArticlesByMuseum(int museumId)
        {
            var response = new BaseEntityResponse<Article>();

            var museum = GetEntityQuery(x => x.Id == museumId);

            response.TotalRecords = await museum.Select(s=> s.Articles).CountAsync();
            response.Items = (List<Article>?)museum.Select(s => s.Articles).AsNoTracking();

            return response;
        }

        /// <summary>
        /// Returns the Museums attending to the Given Filters
        /// </summary>
        /// <param name="filters">Filters to Apply</param>
        /// <returns>A List of Museus</returns>
        public async Task<BaseEntityResponse<Museum>> ListMuseums(BaseFiltersRequest filters)
        {
            var response = new BaseEntityResponse<Museum>();

            var museums = GetEntityQuery(x => x.DeletedAt == null);

            if (!String.IsNullOrEmpty(filters.TextFilter))
            {
                museums = museums.Where(w => w.Name!.Contains(filters.TextFilter));
            }

            if (!String.IsNullOrEmpty(filters.StartDate) && !String.IsNullOrEmpty(filters.EndDate))
            {
                museums = museums.Where(w => w.CreatedAt >= Convert.ToDateTime(filters.StartDate) && w.CreatedAt <= Convert.ToDateTime(filters.EndDate));
            }

            if (filters.Sort is null)
                filters.Sort = "Id";

            response.TotalRecords = await museums.CountAsync();
            response.Items = await Ordering(filters, museums).ToListAsync();

            return response;
        }

        /// <summary>
        /// Returns the Museums in the Table
        /// </summary>
        /// <returns>A List of All Museums</returns>
        public async Task<BaseEntityResponse<Museum>> ListAllMuseums()
        {
            var response = new BaseEntityResponse<Museum>();

            var museums = GetEntityQuery(x => x.DeletedAt == null);

            response.TotalRecords = await museums.CountAsync();
            response.Items = await museums.ToListAsync();

            return response;
        }

        /// <summary>
        /// Returns All Museums By the Theme specified
        /// </summary>
        /// <param name="theme">Theme to List the Museums</param>
        /// <returns>List of All Museums with the specified Theme</returns>
        public async Task<BaseEntityResponse<Museum>> ListMuseumsByTheme(int theme)
        {
            var response = new BaseEntityResponse<Museum>();

            var museums = GetEntityQuery(x => x.DeletedAt == null && x.Theme == theme);

            response.TotalRecords = await museums.CountAsync();
            response.Items = await museums.ToListAsync();

            return response;
        }
    }
}
