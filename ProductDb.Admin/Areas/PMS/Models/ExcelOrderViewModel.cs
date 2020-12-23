using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Areas.PMS.Models
{
    public class ExcelOrderViewModel
    {
        public string IsInvoiced { get; set; }
        public string OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Cost { get; set; }
        public string CurrencyCost { get; set; }
        public string SupplierCode { get; set; }
        public string Quantity { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Phone1 { get; set; }
        public string CorporateName { get; set; }
        public bool IsCanceled { get; set; }
        public string EMailAddress { get; set; }
        public string ProjectCode { get; set; }
        public string CargoBarcode { get; set; }
        public string Price { get; set; }
        public string ShippingName { get; set; }
        public string ShippingAddress1 { get; set; }


        // Güncelleme
        public string Weight { get; set; } // Desi
        public string UnitPrice { get; set; } // Birim Fiyat
        public string Currency { get; set; } // Kur
        public string VatRate { get; set; } // Vergi Oranı

        // Güncelleme 2
        public string TaxNumber { get; set; } // Desi
        public string TaxOffice { get; set; } // Birim Fiyat
        public string CustomerCode { get; set; } // Kur
    }
}
