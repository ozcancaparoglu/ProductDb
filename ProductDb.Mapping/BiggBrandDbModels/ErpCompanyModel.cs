using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class ErpCompanyModel : EntityBaseModel
    {
        public int ErpRef { get; set; }
        public int FirmNo { get; set; }
        public string FirmName { get; set; }
        public string Title { get; set; }
    }
}
