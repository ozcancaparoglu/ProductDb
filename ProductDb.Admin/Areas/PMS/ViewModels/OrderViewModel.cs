using PMS.Common.Dto;
using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Areas.PMS.ViewModels
{
    public class OrderViewModel
    {
        public IEnumerable<ErpCompanyModel> Firms { get; set; }
    }
}
