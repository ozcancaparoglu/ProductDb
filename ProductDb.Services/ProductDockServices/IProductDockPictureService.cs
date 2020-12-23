using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Services.ProductDockServices
{
    public interface IProductDockPictureService
    {
        ProductDockPicturesModel ProductDockPictureByProductDockId(int id);
    }
}
