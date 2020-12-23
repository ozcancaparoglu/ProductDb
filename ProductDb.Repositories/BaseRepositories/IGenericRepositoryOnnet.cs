using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductDb.Repositories.BaseRepositories
{
    public interface IGenericRepositoryOnnet<T> where T : class
    {
        IQueryable<T> ExecuteRawQuery(string query);
    }
}
