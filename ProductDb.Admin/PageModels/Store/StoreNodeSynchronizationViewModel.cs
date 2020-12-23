using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.Store
{
    public class StoreNodeSynchronizationViewModel
    {
        [Display(Name = "Dosya Seçiniz : ")]
        [Required(ErrorMessage = "Dosya Seçmelisiniz")]
        public IFormFile file { get; set; }

        public string message { get; set; }
    }
}
