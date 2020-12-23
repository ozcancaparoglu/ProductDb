using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.ProductDock
{
    public class ChangeParentProductDockViewModel
    {
        public int id { get; set; }
        public List<ParentProductDockModel> ParentProductDocks { get; set; }
    }
}
