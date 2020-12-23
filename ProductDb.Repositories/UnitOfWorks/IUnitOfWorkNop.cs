using ProductDb.Repositories.BaseRepositories;
using System.Threading.Tasks;

namespace ProductDb.Repositories.UnitOfWorks
{
    public interface IUnitOfWorkNop
    {
        IGenericRepositoryNop<T> Repository<T>() where T : class;
        int Commit();
        Task<int> CommitAsync();
        void Rollback();
    }
}