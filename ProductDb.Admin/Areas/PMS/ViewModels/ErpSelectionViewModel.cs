using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Areas.PMS.ViewModels
{
    public class ErpSelectionViewModel
    {
        public List<ErpCompanyModel> ErpCompanies { get; set; }
        public ErpCompanyModel ErpCompanyModel { get; set; }
        public int ErpCompanyNo { get; set; }
    }
}
