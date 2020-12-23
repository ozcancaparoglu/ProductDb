using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Services.BrandServices
{
    public interface IBrandService
    {
        ICollection<BrandModel> AllActiveBrands();
        ICollection<BrandModel> AllBrands();
        BrandModel BrandById(int id);
        BrandModel AddNewBrand(BrandModel model);
        BrandModel EditBrand(BrandModel model);
        void SetState(int objectId);
        BrandModel IsDefined(string name);
    }
}
