using System.Collections.Generic;
using System.Linq;
using ProductDb.Common.Enums;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using Remotion.Linq.Clauses;

namespace ProductDb.Services.WarehouseServices
{
    public class WarehouseQueryService : IWarehouseQueryService
    {
        private IAutoMapperConfiguration autoMapper;
        private IUnitOfWork unitOfWork;
        private IGenericRepository<WarehouseQuery> genericRepositoryLogoWareHouseQuery;
        private IGenericRepository<WarehouseType> genericRepositoryWarehouseType;

        public WarehouseQueryService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper)
        {
            this.autoMapper = autoMapper;
            this.unitOfWork = unitOfWork;
            this.genericRepositoryLogoWareHouseQuery = unitOfWork.Repository<WarehouseQuery>();
            this.genericRepositoryWarehouseType = unitOfWork.Repository<WarehouseType>();

        }

        public WarehouseQueryModel AddLogoQuery(WarehouseQueryModel logoWarehouseQueryModel)
        {
            var InsertedData = genericRepositoryLogoWareHouseQuery.Add(autoMapper.MapObject<WarehouseQueryModel, WarehouseQuery>(logoWarehouseQueryModel));
            return autoMapper.MapObject<WarehouseQuery, WarehouseQueryModel>(InsertedData);
        }

        public int DeleteLogoQuery(WarehouseQueryModel logoWarehouseQueryModel)
        {
            return genericRepositoryLogoWareHouseQuery.Delete(autoMapper.MapObject<WarehouseQueryModel, WarehouseQuery>(logoWarehouseQueryModel));
        }

        public WarehouseQueryModel EditLogoQuery(WarehouseQueryModel logoWarehouseQueryModel)
        {
            var EditedData = genericRepositoryLogoWareHouseQuery.Update(autoMapper.MapObject<WarehouseQueryModel, WarehouseQuery>(logoWarehouseQueryModel));
            return autoMapper.MapObject<WarehouseQuery, WarehouseQueryModel>(EditedData);
        }


        public WarehouseQueryModel GetLogoWarehouseQueryModelByWarehouse(int WarehouseTypeId, List<string> SKUs, int? AMBAR_ID = null)
        {
            var query = genericRepositoryLogoWareHouseQuery.Filter(a => a.WarehouseTypeId == WarehouseTypeId).FirstOrDefault();

            if (query != null)
            {
                var mapModel = autoMapper.MapObject<WarehouseQuery, WarehouseQueryModel>(query);

                var formattedSkus = string.Format("'{0}'", string.Join("','", SKUs.Select(i => i.Replace("'", "''"))));

                string SqlQuery = string.Empty;

                SqlQuery = query.Query + $" WHERE [STOK KODU] IN ({formattedSkus}) GROUP BY [STOK KODU], [AMBAR ADI]";


                //if (AMBAR_ID != null)
                //    SqlQuery = query.Query + " WHERE MLZ_KOD IN(" + formattedSkus + ") AND AMBAR_ID = '" + AMBAR_ID.ToString() + "' GROUP BY MLZ_KOD,AMBAR_ID";
                //else
                //    SqlQuery = query.Query + " WHERE MLZ_KOD IN(" + formattedSkus + ") GROUP BY MLZ_KOD";

                mapModel.Query = SqlQuery;

                return mapModel;
            }
            else
            {
                return null;
            }
        }

        public WarehouseQueryModel GetLogoWarehouseQueryModelByWarehouse(List<string> SKUs, int? AMBAR_ID = null)
        {
            var Type = genericRepositoryWarehouseType.Filter(a => a.LogoWarehouseId == AMBAR_ID).FirstOrDefault();

            var query = genericRepositoryLogoWareHouseQuery.Filter(a => a.WarehouseTypeId == Type.Id).FirstOrDefault();

            var mapModel = autoMapper.MapObject<WarehouseQuery, WarehouseQueryModel>(query);

            var formattedSkus = string.Format("'{0}'", string.Join("','", SKUs.Select(i => i.Replace("'", "''"))));

            string SqlQuery = string.Empty;

            SqlQuery = query.Query + $" WHERE [STOK KODU] IN ({formattedSkus}) GROUP BY [STOK KODU], [AMBAR ADI]";

            //if (AMBAR_ID != null)
            //    SqlQuery = query.Query + " WHERE MLZ_KOD IN(" + formattedSkus + ") AND AMBAR_ID = '" + AMBAR_ID.ToString() + "' GROUP BY MLZ_KOD,AMBAR_ID";

            SqlQuery = string.Format(SqlQuery, Type.Id);
            mapModel.Query = SqlQuery;

            return mapModel;
        }

        public WarehouseQueryModel GetQueryById(int id)
        {
            var data = genericRepositoryLogoWareHouseQuery.GetById(id);
            return autoMapper.MapObject<WarehouseQuery, WarehouseQueryModel>(data);
        }

        public WarehouseQueryModel GetWarehouseQueriesByWarehouseType(int id)
        {
            return autoMapper.MapObject<WarehouseQuery, WarehouseQueryModel>(genericRepositoryLogoWareHouseQuery.Table().FirstOrDefault(x => x.WarehouseTypeId == id));
        }

        public void SetWarehoseTypeState(int objectId)
        {
            var entity = genericRepositoryLogoWareHouseQuery.GetById(objectId);

            if (entity.State == (int)State.Active)
                entity.State = (int)State.Passive;

            else
                entity.State = (int)State.Active;

            genericRepositoryLogoWareHouseQuery.Update(entity);
        }

        public IList<WarehouseQueryModel> WarehouseQueryModels()
        {
            var queries = genericRepositoryLogoWareHouseQuery.GetAll();
            return autoMapper.MapCollection<WarehouseQuery, WarehouseQueryModel>(queries).ToList();
        }

    }
}
