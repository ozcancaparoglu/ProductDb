using ProductDb.Repositories.BaseRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductDb.Repositories.UnitOfWorks
{
    public interface IUnitOfWorkOnnet
    {
        IGenericRepositoryOnnet<T> Repository<T>() where T : class;
        int Commit();
        Task<int> CommitAsync();
        void Rollback();
    }
}
