using Microsoft.EntityFrameworkCore;
using PMS.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Data.Repository
{
    public class Repository<T>: IRepository<T> where T: BaseEntity
    {
        private readonly BiggOfficeContext context;
        public Repository(BiggOfficeContext context)
        {
            this.context = context;
        }

        public IQueryable<T> Table()
        {
            return context.Set<T>().AsQueryable();
        }

        public ICollection<T> GetAll()
        {
            return context.Set<T>().ToList();
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }

        public T GetById(object id)
        {
            return context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public T Find(Expression<Func<T, bool>> match)
        {
            return context.Set<T>().SingleOrDefault(match);
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>().Where(predicate);
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await context.Set<T>().SingleOrDefaultAsync(match);
        }

        public ICollection<T> FindAll(Expression<Func<T, bool>> match)
        {
            return context.Set<T>().Where(match).ToList();
        }

        public async Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await context.Set<T>().Where(match).ToListAsync();
        }

        public T Add(T entity)
        {
            context.Set<T>().Add(entity);
            context.SaveChanges();
            return entity;
        }

        public int AddRange(List<T> entities)
        {
            context.Set<T>().AddRange(entities);
            return context.SaveChanges();
        }

        public T Update(T updated)
        {

            var local = context.Set<T>().Local.FirstOrDefault(entry => entry.Id.Equals(updated.Id));
            if (local != null)
                context.Entry(local).State = EntityState.Detached;

            context.Set<T>().Attach(updated);
            context.Entry(updated).State = EntityState.Modified;

            context.SaveChanges();

            return updated;
        }

        public void UpdateRange(List<T> entities)
        {
            context.Set<T>().AttachRange(entities);

            foreach (T entity in entities)
            {
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
            context.SaveChanges();
        }

        public int DeleteRange(List<T> entities)
        {
            context.Set<T>().RemoveRange(entities);
            return context.SaveChanges();
        }

        public virtual int Count()
        {
            return context.Set<T>().Count();
        }

        public virtual async Task<int> CountAsync()
        {
            return await context.Set<T>().CountAsync();
        }

        public virtual ICollection<T> Filter(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "", int? page = null, int? pageSize = null)
        {
            IQueryable<T> query = context.Set<T>();

            if (filter != null)
                query = query.Where(filter);

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

            if (orderBy != null)
                query = orderBy(query);

            if (page != null && pageSize != null)
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

            return query.ToList();
        }

        public virtual bool Exist(Expression<Func<T, bool>> predicate)
        {
            var exist = context.Set<T>().Where(predicate);
            return exist.Any() ? true : false;
        }

        public async Task<ICollection<T>> FilterAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", int? page = null, int? pageSize = null)
        {
            IQueryable<T> query = context.Set<T>();

            if (filter != null)
                query = query.Where(filter);

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

            if (orderBy != null)
                query = orderBy(query);

            if (page != null && pageSize != null)
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

            return await query.ToListAsync(); ;
        }
    }
}
