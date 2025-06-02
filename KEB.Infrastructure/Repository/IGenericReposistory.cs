using KEB.Domain.Entities;
using KEB.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository
{
    public interface IGenericReposistory<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, string includeProperties ="",bool asTracking = false);
        Task<ICollection<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = "",
            int pageNumber = 0,
            int pageSize = 0,
             bool asTracking = false,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task<ICollection<T>> GetPaginatedAsync(
            int pageNumber, int pageSize,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task AddAsync(T entity);
        Task AddWithNoCommitAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        Task AddRangeWithNoCommitAsync(IEnumerable<T> entities);

        Task DeleteRangeAsync(IEnumerable<T> entities);
        void Detach(T entity);
        string GetEntityState(T entity);
        Task<ICollection<T>> FindPagedAsync(
            Expression<Func<T, bool>> predicate,
            int pageNumber,
            int pageSize,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task UpdateWithNoCommitAsync(T entity);
        Task RemoveAsync(T entity);
        Task<int> CountAsync<T>(Expression<Func<T, bool>> filter = null) where T : class;
    }
}
