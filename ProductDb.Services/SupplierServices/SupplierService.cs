using ProductDb.Common.Enums;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ProductDb.Services.SupplierServices
{
    public class SupplierService : ISupplierService
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IAutoMapperConfiguration autoMapper;

        private readonly IGenericRepository<Supplier> supplierRepo;

        private readonly Expression<Func<Supplier, bool>> defaultSuppliersFilter = null;

        public SupplierService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper)
        {
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;

            supplierRepo = this.unitOfWork.Repository<Supplier>();

            defaultSuppliersFilter = entity => entity.State == (int)State.Active;
        }

        public ICollection<SupplierModel> AllSuppliers()
        {
            return autoMapper.MapCollection<Supplier, SupplierModel>(supplierRepo.GetAll()).OrderBy(x => x.Name).ToList();
        }
        public ICollection<SupplierModel> AllActiveSuppliers()
        {
            return autoMapper.MapCollection<Supplier, SupplierModel>(supplierRepo.FindAll(defaultSuppliersFilter)).OrderBy(x => x.Name).ToList();
        }

        public SupplierModel SupplierById(int id)
        {
            return autoMapper.MapObject<Supplier, SupplierModel>(supplierRepo.GetById(id));
        }

        public SupplierModel AddNewSupplier(SupplierModel model)
        {
            var entity = autoMapper.MapObject<SupplierModel, Supplier>(model);

            #region Validations

            if (supplierRepo.Filter(x => x.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant()).Count() > 0)
                return null;

            //if (supplierRepo.Filter(x => x.ManufacturerPartNumber == entity.ManufacturerPartNumber).Count() > 0)
            //    return null;

            #endregion

            var savedEntity = supplierRepo.Add(entity);

            return autoMapper.MapObject<Supplier, SupplierModel>(savedEntity);
        }

        public SupplierModel EditSupplier(SupplierModel model)
        {
            var entity = autoMapper.MapObject<SupplierModel, Supplier>(model);

            #region Validations

            if (supplierRepo.Filter(x => x.Id != entity.Id && x.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant()).Count() > 0)
                return null;

            //if (supplierRepo.Filter(x => x.Id != entity.Id && x.ManufacturerPartNumber == entity.ManufacturerPartNumber).Count() > 0)
            //    return null;

            #endregion

            var savedEntity = supplierRepo.Update(entity);

            return autoMapper.MapObject<Supplier, SupplierModel>(savedEntity);
        }

        public void SetState(int objectId)
        {
            var entity = supplierRepo.GetById(objectId);

            if (entity.State == (int)State.Active)
                entity.State = (int)State.Passive;

            else
                entity.State = (int)State.Active;

            supplierRepo.Update(entity);
        }

    }
}
