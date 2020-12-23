using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.Transportation
{
    public class TransportationViewModel
    {
        public StoreModel StoreModel { get; set; }
        public TransportationTypeModel TransportationTypeModel { get; set; }
        public List<CurrencyModel> Currencies { get; set; }
        public List<TransportationTypeModel> TransportationTypes { get; set; }
    }
}
