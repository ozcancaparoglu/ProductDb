using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductDb.Data.OnnetDb;
using ProductDb.Repositories.BaseRepositories;

namespace ProductDb.Repositories.UnitOfWorks
{
    public class UnitOfWorkOnnet : IUnitOfWorkOnnet
    {
        private readonly OnnetDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public Dictionary<Type, object> Repositories
        {
            get { return _repositories; }
            set { Repositories = value; }
        }

        public UnitOfWorkOnnet(OnnetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IGenericRepositoryOnnet<T> Repository<T>() where T : class
        {
            if (Repositories.Keys.Contains(typeof(T)))
            {
                return Repositories[typeof(T)] as IGenericRepositoryOnnet<T>;
            }

            IGenericRepositoryOnnet<T> repo = new GenericRepositoryOnnet<T>(_dbContext);
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
