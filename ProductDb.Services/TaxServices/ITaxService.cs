using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections;
using System.Collections.Generic;

namespace ProductDb.Services.TaxServices
{
    public interface ITaxService
    {
        ICollection<VatRateModel> AllTaxRate();
    }
}
