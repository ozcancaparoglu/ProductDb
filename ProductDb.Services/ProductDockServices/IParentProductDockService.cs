using ProductDb.Data.BiggBrandsDb.ProductDocks;
using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Services.ProductDockServices
{
    public interface IParentProductDockService
    {
        ICollection<ParentProductDockModel> AllParentProductDocks();
        ParentProductDockModel AddNewParentProductDock(ParentProductDockModel model);
        ParentProductDockModel ParentProductDockById(int id);
        ParentProductDockModel ParentProductDockBySku(string sku);
    }
}
