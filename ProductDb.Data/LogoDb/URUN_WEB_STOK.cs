using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.LogoDb
{
    [Table("URUN_WEB_STOK")]
    public class URUN_WEB_STOK
    {
        [Key]
        public long Id { get; set; }
        public string Sku { get; set; }
        public double? Stock { get; set; }
        public string WarehouseName { get; set; }
    }
}
