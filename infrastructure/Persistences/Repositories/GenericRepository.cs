using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Helpers;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly MuseumsDbContext _context;
        private readonly DbSet<T> _entity;

        public GenericRepository(MuseumsDbContext context)
        {
            _context = context;
            _entity = _context.Set<T>();
        }

        /// <summary>
        /// Returns All Items of the Table
        /// </summary>
        /// <returns>All Items of the Table as an Enumerable </returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var getAll = await _entity
                .Where(w => w.DeletedAt == null).AsNoTracking().ToListAsync();

            return getAll;
        }

        /// <summary>
        /// Returns an Item Given its Id
        /// </summary>
        /// <param name="id">Identificator of the Item to Return</param>
        /// <returns>Item corresponding to the Given Identificator</returns>
        public async Task<T> GetByIdAsync(int id)
        {
            var getById = await _entity.AsNoTracking().Where(w => w.Id == id).FirstOrDefaultAsync();

            return getById!;
        }

        /// <summary>
        /// Register a New Item
        /// </summary>
        /// <param name="entity">Item to Register</param>
        /// <returns>Numbers of Rows Affected in the opperation</returns>
        public async Task<bool> RegisterAsync(T entity)
        {
            entity.CreatedAt = DateTime.Now;

            await _context.AddAsync(entity);

            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }

        /// <summary>
        /// Edit an Item Given
        /// </summary>
        /// <param name="entity">Item to Edit</param>
        /// <returns>Numbers of Rows Affected in the opperation</returns>
        public async Task<bool> EditAsync(T entity)
        {
            entity.UpdatedAt = DateTime.Now;

            _context.Update(entity);
            _context.Entry(entity).Property(p => p.CreatedAt).IsModified = false;

            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }

        /// <summary>
        /// Erase completely the Item from the Table. Not Possible to Recover it
        /// </summary>
        /// <param name="id">Id of the Item to Erase</param>
        /// <returns>Numbers of Rows Affected in the opperation</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            T entity = await GetByIdAsync(id);

            _context.Remove(entity);

            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }

        /// <summary>
        /// Removes the Item, updating the DeletedAt field. Possible to Recover it
        /// </summary>
        /// <param name="id">Id of the Item to Erase</param>
        /// <returns>Numbers of Rows Affected in the opperation</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            T entity = await GetByIdAsync(id);

            entity!.DeletedAt = DateTime.Now;

            _context.Update(entity);

            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }

        /// <summary>
        /// Returns the data of the Entity as an IQueryable, to allow queries on it
        /// </summary>
        /// <param name="filter">Filter to apply to the Entity</param>
        /// <returns>Returns the Entity data as an IQueryable</returns>
        public IQueryable<T> GetEntityQuery(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _entity;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query;
        }

        /// <summary>
        /// Sorts the Entity's data and returns it as an IQueryable type, to allow queries against it
        /// </summary>
        /// <typeparam name="TDTO">Generic Data Type</typeparam>
        /// <param name="request">Request nto Order</param>
        /// <param name="queryable"></param>
        /// <param name="pagination">If should have Pagination</param>
        /// <returns>The Data of the Entity ordered according to the given parameters</returns>
        public IQueryable<TDTO> Ordering<TDTO>(BasePaginationRequest request, IQueryable<TDTO> queryable, bool pagination = false) where TDTO : class
        {
            IQueryable<TDTO> queryViewModel = request.Order == "desc" ? queryable.OrderBy($"{request.Sort} descending") : queryable.OrderBy($"{request.Sort} ascending");

            if (pagination)
            {
                queryViewModel = queryViewModel.Paginate(request);
            }

            return queryViewModel;
        }
    }
}
