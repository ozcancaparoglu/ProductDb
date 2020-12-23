using ProductDb.Repositories.BaseRepositories;
using System.Threading.Tasks;

namespace ProductDb.Repositories.UnitOfWorks
{
    public interface IUnitOfWorkLogo
    {
        IGenericRepositoryLogo<T> Repository<T>() where T : class;
        int Commit();
        Task<int> CommitAsync();
        void Rollback();
    }
}