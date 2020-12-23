using Microsoft.EntityFrameworkCore;
using ProductDb.Data.OnnetDb;
using ProductDb.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductDb.Repositories.BaseRepositories
{
    public class GenericRepositoryOnnet<T> : IGenericRepositoryOnnet<T> where T : class
    {
        private OnnetDbContext context;
        private readonly IUnitOfWorkOnnet unitOfWork;

        public GenericRepositoryOnnet(OnnetDbContext context)
        {
            this.context = context;
            unitOfWork = new UnitOfWorkOnnet(context);
        }

        public IQueryable<T> ExecuteRawQuery(string query)
        {
            return context.Set<T>().FromSql(query).AsNoTracking();
        }
    }
}
