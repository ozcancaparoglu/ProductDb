using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Services.AttributesServices
{
    public interface IAttributesService
    {
        ICollection<AttributesModel> AllAttributes();
        ICollection<AttributesModel> AllActiveAttributes();
        ICollection<AttributesModel> AttributesRequired();
        AttributesModel AttributesById(int id);
        AttributesModel AddNewAttributes(AttributesModel model);
        AttributesModel EditAttributes(AttributesModel model);
        ICollection<AttributesModel> AllAttributesExcept(List<AttributesModel> parentModelList, List<AttributesModel> modelList);
        string AttributeNameById(int id);
        void SetState(int objectId);
        int AttributesCount();

        #region Attribute Values

        ICollection<AttributesValueModel> AllAttributesValue(int attributeId);
        ICollection<AttributesValueModel> AllActiveAttributesValue(int attributeId);
        AttributesValueModel AttributesValueById(int id);
        AttributesValueModel AddNewAttributesValue(AttributesValueModel model);
        AttributesValueModel EditAttributesValue(AttributesValueModel model);
        int AttributeIdWithAttributesValueId(int id);
        string AttributeValueNameById(int id);
        void SetStateAttributesValue(int objectId);
        ICollection<AttributesModel> AllVariantableAttributes();
        #endregion
    }
}