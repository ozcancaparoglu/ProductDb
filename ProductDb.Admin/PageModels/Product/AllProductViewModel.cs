using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.Product
{
    public class AllProductViewModel
    {
        public int languageId { get; set; }
        public List<LanguageModel> languages { get; set; }
    }
}
