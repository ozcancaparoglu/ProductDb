using ProductDb.Data.NopDb;
using ProductDb.Repositories.BaseRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Repositories.UnitOfWorks
{
    public class UnitOfWorkNop : IUnitOfWorkNop
    {
        private readonly NopDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public Dictionary<Type, object> Repositories
        {
            get { return _repositories; }
            set { Repositories = value; }
        }

        public UnitOfWorkNop(NopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IGenericRepositoryNop<T> Repository<T>() where T : class
        {
            if (Repositories.Keys.Contains(typeof(T)))
            {
                return Repositories[typeof(T)] as IGenericRepositoryNop<T>;
            }

            IGenericRepositoryNop<T> repo = new GenericRepositoryNop<T>(_dbContext);
            Repositories.Add(typeof(T), repo);
            return repo;
        }

        public int Commit()
        {
            return _dbContext.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Rollback()
        {
            _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        }
    }
}
