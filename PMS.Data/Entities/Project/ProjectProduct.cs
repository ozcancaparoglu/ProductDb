using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Data.Entities.Project
{
    public class ProjectProduct: BaseEntity
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string CatalogCode { get; set; }
        public string LogoCode { get; set; }
        public string CatalogDescription { get; set; }
        public int? TaxRate { get; set; }
        public decimal? Consumable { get; set; }
        public string ConsumableCurrency { get; set; }
        public decimal? Price { get; set; }
        public string PriceCurrency { get; set; }
        public int? ProductWeight { get; set; }
        public string GroupCode { get; set; }
        public decimal? CargoPrice { get; set; }
        public string CargoCurrency { get; set; }
    }
}
