using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using ProductDb.Common.Entities;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProductDb.Repositories.BaseRepositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : EntityBase
    {
        private readonly BiggBrandsDbContext context;
        private readonly IUnitOfWork unitOfWork;

        public GenericRepository(BiggBrandsDbContext context)
        {
            this.context = context;
            unitOfWork = new UnitOfWork(context);
        }


        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return context.Database.ExecuteSqlCommand(sql, parameters);
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

        public T GetById(int id)
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
            unitOfWork.Commit();
            return entity;
        }

        public int AddRange(List<T> entities)
        {
            context.Set<T>().AddRange(entities);
            return unitOfWork.Commit();
        }

        public T Update(T updated)
        {
            var local = context.Set<T>().Local
               .FirstOrDefault(entry => entry.Id.Equals(updated.Id));
            if (local != null)
                context.Entry(local).State = EntityState.Detached;

            context.Set<T>().Attach(updated);
            context.Entry(updated).State = EntityState.Modified;
            unitOfWork.Commit();
            return updated;
        }

        public void Update(T obj, params Expression<Func<T, object>>[] propertiesToUpdate)
        {
            context.Set<T>().Attach(obj);

            foreach (var p in propertiesToUpdate)
            {
                context.Entry(obj).Property(p).IsModified = true;
            }

            unitOfWork.Commit();
        }

        public void UpdateRange(List<T> entities, params Expression<Func<T, object>>[] propertiesToUpdate) 
        {
            context.Set<T>().AttachRange(entities);

            foreach (T entity in entities)
            {
                foreach (var p in propertiesToUpdate)
                {
                    context.Entry(entity).Property(p).IsModified = true;
                }
            }

            unitOfWork.Commit();

        }

        public int UpdateRange(List<T> entities)
        {
            context.Set<T>().AttachRange(entities);

            foreach (T entity in entities)
            {
                context.Entry(entity).State = EntityState.Modified;
            }

            return unitOfWork.Commit();
        }

        public void BulkUpdate(List<T> entities)
        {
            context.BulkUpdate(entities);
        }

        public void BulkInsertOrUpdate(List<T> entities)
        {
            context.BulkInsertOrUpdate(entities);
        }

        public void BulkInsert(List<T> entities)
        {
            context.BulkInsert(entities);
        }

        public void BulkDelete(List<T> entities)
        {
            context.BulkDelete(entities);
        }

        public int Delete(T entity)
        {
            var local = context.Set<T>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(entity.Id));
            if (local != null)
                context.Entry(local).State = EntityState.Detached;

            context.Set<T>().Remove(entity);
            return unitOfWork.Commit();
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

    }
}
