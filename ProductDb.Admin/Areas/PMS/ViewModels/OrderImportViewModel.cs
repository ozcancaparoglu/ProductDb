using Microsoft.AspNetCore.Http;
using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Areas.PMS.ViewModels
{
    public class OrderImportViewModel
    {
        public int StoreId { get; set; }
        public List<StoreModel> Stores { get; set; }
        public StoreModel Store { get; set; }
        public IFormFile formFile { get; set; }
    }
}
