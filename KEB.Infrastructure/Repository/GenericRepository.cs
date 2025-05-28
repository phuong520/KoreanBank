using KEB.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericReposistory<T> where T : class
    {
        protected readonly ExamBankContext _context;

        public GenericRepository(ExamBankContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            //await _context.SaveChangesAsync();
        }

        public async Task AddRangeWithNoCommitAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public async Task AddWithNoCommitAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

      
        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public void Detach(T entity)
        {
             _context.Entry(entity).State = EntityState.Detached;
        }

        public async Task<ICollection<T>> FindPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            if(orderBy != null)
            {
                query = orderBy(query);
            }
            query  = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return await query.ToListAsync();
        }

        public async Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, string includeProperties = "", int pageNumber = 0, int pageSize = 0, bool asTracking = false, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (asTracking)
            {
                query = query.AsTracking();
            }
            else
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            foreach(var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp.Trim());
            }
            if(orderBy != null)
            {
                query = orderBy(query);
            }
            if(pageNumber > 0 && pageSize > 0)
            {
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }
            return await query.ToListAsync();
        }


        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, string includeProperties = "", bool asTracking = false)
        {
            IQueryable<T> query = _context.Set<T>();
            if (asTracking)
            {
                query = query.AsTracking();
            }
            else
            {
                query = query.AsNoTracking();
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includePro in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includePro.Trim());
                }
            }
            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public string GetEntityState(T entity)
        {
           return _context.Entry(entity).State.ToString();
        }

        public async Task<ICollection<T>> GetPaginatedAsync(int pageNumber, int pageSize, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if(orderBy != null)
            {
                query = orderBy(query);
            }
            query = query.Skip((pageNumber-1)*pageSize).Take(pageSize);
            return await query.ToListAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _context.Attach(entity);
            }

            _context.Remove(entity);
            // Không gọi SaveChangesAsync ở đây để hỗ trợ transaction
        }

        public Task UpdateWithNoCommitAsync(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }
    }
}
