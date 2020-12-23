using Microsoft.EntityFrameworkCore;
using ProductDb.Common.Enums;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using System.Collections.Generic;
using System.Linq;

namespace ProductDb.Services.WarehouseServices
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAutoMapperConfiguration autoMapper;

        private readonly IGenericRepository<WarehouseProductStock> warehouseProductStockRepo;
        private readonly IGenericRepository<WarehouseType> warehouseTypeRepo;

        public WarehouseService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper)
        {
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;

            warehouseProductStockRepo = this.unitOfWork.Repository<WarehouseProductStock>();
            warehouseTypeRepo = this.unitOfWork.Repository<WarehouseType>();
        }

        public ICollection<WarehouseTypeModel> AllWarehouseTypes()
        {
            return autoMapper.MapCollection<WarehouseType, WarehouseTypeModel>(warehouseTypeRepo.FindAll(x => x.State == (int)State.Active)).ToList();
        }

        #region ProductStockModel

        public IQueryable<WarehouseProductStockModel> WarehouseProductStockModelsQueryable()
        {
            return (warehouseProductStockRepo.Table()
                                .Include(x => x.WarehouseType)
                                .Select(x => new WarehouseProductStockModel()
                                {
                                    Id = x.Id,
                                    Sku = x.Sku,
                                    Name = x.WarehouseType.Name,
                                    ProductName = x.Name,
                                    Quantity = x.Quantity,
                                    WarehouseType = autoMapper.MapObject<WarehouseType, WarehouseTypeModel>(x.WarehouseType),
                                    WarehouseTypeId = x.WarehouseType.Id
                                }).OrderByDescending(x => x.CreatedDate));
        }

        public IEnumerable<WarehouseProductStockModel> WarehouseProductStockModelsQueryableFiltered(int skip, int take, out int total)
        {
            var query = WarehouseProductStockModelsQueryable();
            
            total = query.Count();

            return query.Skip(skip).Take(take).OrderByDescending(x => x.UpdatedDate);
        }

        public ICollection<WarehouseProductStockModel> WarehouseProductStockModels()
        {
            return autoMapper.MapCollection<WarehouseProductStock, WarehouseProductStockModel>(warehouseProductStockRepo.Filter(x => x.State == (int)State.Active, null, "WarehouseType")).ToList();
        }
        public ICollection<WarehouseProductStockModel> WarehouseProductStockModelByWarehouseId(int WarehouseTypeId)
        {
            return autoMapper.MapCollection<WarehouseProductStock, WarehouseProductStockModel>(warehouseProductStockRepo.
                Filter(a => a.WarehouseTypeId == WarehouseTypeId).ToList()).ToList();
        }
        public ICollection<WarehouseTypeModel> WarehouseTypes()
        {
            return autoMapper.MapCollection<WarehouseType, WarehouseTypeModel>(warehouseTypeRepo.FindAll(a => a.State == (int)State.Active)).ToList();
        }

        public void AddNewProductStock(WarehouseProductStockModel warehouseProductStockModel)
        {
            var warehouseTypes = WarehouseTypes();
            foreach (var item in warehouseTypes)
            {
                warehouseProductStockModel.WarehouseTypeId = item.Id;
                warehouseProductStockRepo.Add(autoMapper.MapObject<WarehouseProductStockModel, WarehouseProductStock>(warehouseProductStockModel));
            }
        }

        public void EditProductStock(WarehouseProductStockModel warehouseProductStockModel)
        {
            var ProductList = warehouseProductStockRepo.Filter(a => a.ProductId == warehouseProductStockModel.ProductId).ToList();

            if (ProductList.Count > 0)
            {
                ProductList.ForEach(a => a.Sku = warehouseProductStockModel.Sku);
                warehouseProductStockRepo.UpdateRange(ProductList);
            }
        }
        public bool IsProductStockDefined(int ProductId)
        {
            return warehouseProductStockRepo.Filter(a => a.ProductId == ProductId).Count == 0 ? false : true;
        }
        #endregion

        #region WarehouseType
        public WarehouseTypeModel AddWarehouseType(WarehouseTypeModel model)
        {
            var insertedModel = warehouseTypeRepo.Add(autoMapper.MapObject<WarehouseTypeModel, WarehouseType>(model));
            return autoMapper.MapObject<WarehouseType, WarehouseTypeModel>(insertedModel);
        }

        public WarehouseTypeModel EditWarehouseType(WarehouseTypeModel model)
        {
            var editedModel = warehouseTypeRepo.Update(autoMapper.MapObject<WarehouseTypeModel, WarehouseType>(model));
            return autoMapper.MapObject<WarehouseType, WarehouseTypeModel>(editedModel);
        }

        public WarehouseTypeModel WarehouseTypeById(int id)
        {
            var model = warehouseTypeRepo.GetById(id);
            return autoMapper.MapObject<WarehouseType, WarehouseTypeModel>(model);
        }

        public void SetWarehoseTypeState(int objectId)
        {
            var entity = warehouseTypeRepo.GetById(objectId);

            if (entity.State == (int)State.Active)
                entity.State = (int)State.Passive;

            else
                entity.State = (int)State.Active;

            warehouseTypeRepo.Update(entity);
        }

        public ICollection<WarehouseProductStockModel> WarehouseProductStockModels(int productId)
        {
            return autoMapper.MapCollection<WarehouseProductStock, WarehouseProductStockModel>(warehouseProductStockRepo.Filter(x => x.State == (int)State.Active && x.ProductId == productId, null, "WarehouseType")).ToList();
        }

        #endregion

    }
}
