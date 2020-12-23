using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Data.Repository
{
    public interface IRepository<T> where T: class
    {
        IQueryable<T> Table();
        ICollection<T> GetAll();
        Task<ICollection<T>> GetAllAsync();
        T GetById(object id);
        Task<T> GetByIdAsync(int id);
        T Find(Expression<Func<T, bool>> match);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        ICollection<T> FindAll(Expression<Func<T, bool>> match);
        Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match);
        T Add(T entity);
        int AddRange(List<T> entities);
        T Update(T updated);
        void UpdateRange(List<T> entities);
        void Delete(T entity);
        int DeleteRange(List<T> entities);
        int Count();
        Task<int> CountAsync();
        ICollection<T> Filter(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "", int? page = null, int? pageSize = null);

        Task<ICollection<T>> FilterAsync(Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string includeProperties = "", int? page = null, int? pageSize = null);
        bool Exist(Expression<Func<T, bool>> predicate);

    }
}
