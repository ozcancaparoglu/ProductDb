using ProductDb.Common.Entities;
using ProductDb.Repositories.BaseRepositories;
using System.Threading.Tasks;

namespace ProductDb.Repositories.UnitOfWorks
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> Repository<T>() where T : EntityBase;
        int Commit();
        Task<int> CommitAsync();
        void Rollback();
    }
}