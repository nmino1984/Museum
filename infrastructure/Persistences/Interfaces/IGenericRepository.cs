using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<bool> RegisterAsync(T entity);

        /// <summary>
        /// Persists a collection of entities in a single database round-trip.
        /// <c>CreatedAt</c> is set automatically for every item before insertion.
        /// </summary>
        /// <param name="entities">Collection of entities to insert.</param>
        /// <returns><c>true</c> if all rows were saved successfully; otherwise <c>false</c>.</returns>
        Task<bool> BulkRegisterAsync(IEnumerable<T> entities);

        Task<bool> EditAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> RemoveAsync(int id);
        IQueryable<T> GetEntityQuery(Expression<Func<T, bool>>? filter = null);
        IQueryable<TDTO> Ordering<TDTO>(BasePaginationRequest request, IQueryable<TDTO> queryable, bool pagination = false) where TDTO : class;
    }
}
