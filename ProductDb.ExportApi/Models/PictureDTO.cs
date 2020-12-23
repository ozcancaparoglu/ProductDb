using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.ExportApi.DTOs
{
    public class PictureDTO
    {
        //public int? ProductId { get; set; }
        //public string LocalPath { get; set; }
        public string CdnPath { get; set; }
        //public string Alt { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        //public string Sku { get; set; }
    }
}
