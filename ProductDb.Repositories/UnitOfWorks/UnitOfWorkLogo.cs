using ProductDb.Data.LogoDb;
using ProductDb.Repositories.BaseRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Repositories.UnitOfWorks
{
    public class UnitOfWorkLogo : IUnitOfWorkLogo
    {
        private readonly LogoDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public Dictionary<Type, object> Repositories
        {
            get { return _repositories; }
            set { Repositories = value; }
        }

        public UnitOfWorkLogo(LogoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IGenericRepositoryLogo<T> Repository<T>() where T : class
        {
            if (Repositories.Keys.Contains(typeof(T)))
            {
                return Repositories[typeof(T)] as IGenericRepositoryLogo<T>;
            }

            IGenericRepositoryLogo<T> repo = new GenericRepositoryLogo<T>(_dbContext);
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
