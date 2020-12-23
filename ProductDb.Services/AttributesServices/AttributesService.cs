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

namespace ProductDb.Services.AttributesServices
{
    public class AttributesService : IAttributesService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAutoMapperConfiguration autoMapper;

        private readonly IGenericRepository<Attributes> attributesRepo;
        private readonly IGenericRepository<AttributesValue> attributesValueRepo;

        private readonly Expression<Func<Attributes, bool>> defaultAttributesFilter = null;

        public AttributesService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper)
        {
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;

            attributesRepo = this.unitOfWork.Repository<Attributes>();
            attributesValueRepo = this.unitOfWork.Repository<AttributesValue>();

            defaultAttributesFilter = entity => entity.State == (int)State.Active;
        }

        public ICollection<AttributesModel> AllAttributes()
        {
            return autoMapper.MapCollection<Attributes, AttributesModel>(attributesRepo.GetAll()).ToList();
        }

        public ICollection<AttributesModel> AllActiveAttributes()
        {
            return autoMapper.MapCollection<Attributes, AttributesModel>(attributesRepo.FindAll(defaultAttributesFilter)).ToList();
        }

        public ICollection<AttributesModel> AttributesRequired()
        {
            return autoMapper.MapCollection<Attributes, AttributesModel>(attributesRepo.FindAll(x => x.IsRequired && x.State == (int)State.Active)).ToList();
        }

        public AttributesModel AttributesById(int id)
        {
            return autoMapper.MapObject<Attributes, AttributesModel>(attributesRepo.GetById(id));
        }

        public AttributesModel AddNewAttributes(AttributesModel model)
        {
            var entity = autoMapper.MapObject<AttributesModel, Attributes>(model);

            if (attributesRepo.Exist(x => x.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant()))
                return null;

            var savedEntity = attributesRepo.Add(entity);

            return autoMapper.MapObject<Attributes, AttributesModel>(savedEntity);

        }

        public AttributesModel EditAttributes(AttributesModel model)
        {
            var entity = autoMapper.MapObject<AttributesModel, Attributes>(model);

            if (attributesRepo.Filter(x => x.Id != entity.Id && x.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant()).Count() > 0)
                return null;

            var updatedEntity = attributesRepo.Update(entity);

            return autoMapper.MapObject<Attributes, AttributesModel>(updatedEntity);
        }

        public ICollection<AttributesModel> AllAttributesExcept(List<AttributesModel> parentModelList, List<AttributesModel> modelList)
        {
            modelList.AddRange(parentModelList);

            var entities = attributesRepo.Filter(x => !modelList.Select(ml => ml.Id).Contains(x.Id) && x.IsRequired == false && x.State == (int)State.Active);

            return autoMapper.MapCollection<Attributes, AttributesModel>(entities).ToList();
        }

        public string AttributeNameById(int id)
        {
            return attributesRepo.GetById(id).Name;
        }

        public void SetState(int objectId)
        {
            var entity = attributesRepo.GetById(objectId);

            if (entity.State == (int)State.Active)
            {
                entity.State = (int)State.Passive;
                foreach (var attributeValue in attributesValueRepo.Filter(x => x.AttributesId == entity.Id).ToList())
                {
                    attributeValue.State = (int)State.Passive;
                    attributesValueRepo.Update(attributeValue);
                }
            }
            else
            {
                entity.State = (int)State.Active;
                foreach (var attributeValue in attributesValueRepo.Filter(x => x.AttributesId == entity.Id).ToList())
                {
                    attributeValue.State = (int)State.Active;
                    attributesValueRepo.Update(attributeValue);
                }
            }
                

            attributesRepo.Update(entity);
        }

        public int AttributesCount()
        {
            return attributesRepo.Count();
        }

        #region Attribute Values

        public ICollection<AttributesValueModel> AllAttributesValue(int attributeId)
        {
            return autoMapper.MapCollection<AttributesValue, AttributesValueModel>(attributesValueRepo.Filter(x => x.AttributesId == attributeId)).ToList();
        }

        public ICollection<AttributesValueModel> AllActiveAttributesValue(int attributeId)
        {
            return autoMapper.MapCollection<AttributesValue, AttributesValueModel>(attributesValueRepo.FindAll(x => x.AttributesId == attributeId && x.State == (int)State.Active)).ToList();
        }

        public AttributesValueModel AttributesValueById(int id)
        {
            return autoMapper.MapObject<AttributesValue, AttributesValueModel>(attributesValueRepo.GetById(id));
        }

        public AttributesValueModel AddNewAttributesValue(AttributesValueModel model)
        {
            var entity = autoMapper.MapObject<AttributesValueModel, AttributesValue>(model);

            if (attributesValueRepo.Exist(x => x.Value.ToLowerInvariant() == entity.Value.ToLowerInvariant() && x.AttributesId == entity.AttributesId))
                return null;

            var savedEntity = attributesValueRepo.Add(entity);

            return autoMapper.MapObject<AttributesValue, AttributesValueModel>(savedEntity);

        }

        public AttributesValueModel EditAttributesValue(AttributesValueModel model)
        {
            var entity = autoMapper.MapObject<AttributesValueModel, AttributesValue>(model);

            var updatedEntity = attributesValueRepo.Update(entity);

            return autoMapper.MapObject<AttributesValue, AttributesValueModel>(updatedEntity);
        }

        public int AttributeIdWithAttributesValueId(int id)
        {
            return attributesValueRepo.GetById(id).AttributesId ?? 0;
        }

        public string AttributeValueNameById(int id)
        {
            return attributesValueRepo.GetById(id).Value;
        }

        public void SetStateAttributesValue(int objectId)
        {
            var entity = attributesValueRepo.GetById(objectId);

            if (entity.State == (int)State.Active)
                entity.State = (int)State.Passive;
            else
                entity.State = (int)State.Active;

            attributesValueRepo.Update(entity);
        }

        public ICollection<AttributesModel> AllVariantableAttributes()
        {
            return autoMapper.MapCollection<Attributes, AttributesModel>(attributesRepo.Filter(x => x.IsVariantable)).ToList();
        }

        #endregion


    }
}
