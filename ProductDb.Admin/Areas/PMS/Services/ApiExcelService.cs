using System;
using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
using ProductDb.Admin.Areas.PMS.Models;

namespace ProductDb.Admin.Areas.PMS.Services
{
    public class ApiExcelService : IApiExcelService
    {
        public IList<ExcelOrderViewModel> ReadOrders(string path)
        {
            var orderList = new List<ExcelOrderViewModel>();

            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    reader.Read(); // skip first row

                    while (reader.Read())
                    {
                        ExcelOrderViewModel row = new ExcelOrderViewModel();

                        row.IsInvoiced = reader.GetValue(0) == null ? string.Empty : Convert.ChangeType(reader.GetValue(0), reader.GetValue(0).GetType()).ToString();
                        row.CargoBarcode = reader.GetValue(1) == null ? string.Empty : Convert.ChangeType(reader.GetValue(1), reader.GetValue(1).GetType()).ToString();
                        row.OrderDate = reader.GetValue(2) == null ? string.Empty : Convert.ChangeType(reader.GetValue(2), reader.GetValue(2).GetType()).ToString();
                        row.OrderNumber = reader.GetValue(3) == null ? string.Empty : Convert.ChangeType(reader.GetValue(3), reader.GetValue(3).GetType()).ToString();
                        row.ProductCode = reader.GetValue(4) == null ? string.Empty : Convert.ChangeType(reader.GetValue(4), reader.GetValue(4).GetType()).ToString();
                        row.ProductName = reader.GetValue(5) == null ? string.Empty : Convert.ChangeType(reader.GetValue(5), reader.GetValue(5).GetType()).ToString();
                        row.CurrencyCost = reader.GetValue(6) == null ? string.Empty : Convert.ChangeType(reader.GetValue(6), reader.GetValue(6).GetType()).ToString().Trim();
                        row.Price = reader.GetValue(7) == null ? string.Empty : Convert.ChangeType(reader.GetValue(7), reader.GetValue(7).GetType()).ToString();
                        row.SupplierCode = reader.GetValue(8) == null ? string.Empty : Convert.ChangeType(reader.GetValue(8), reader.GetValue(8).GetType()).ToString();
                        row.Quantity = reader.GetValue(9) == null ? string.Empty : Convert.ChangeType(reader.GetValue(9), reader.GetValue(9).GetType()).ToString();
                        row.Address = reader.GetValue(10) == null ? string.Empty : Convert.ChangeType(reader.GetValue(10), reader.GetValue(10).GetType()).ToString();
                        row.CustomerName = reader.GetValue(11) == null ? string.Empty : Convert.ChangeType(reader.GetValue(11), reader.GetValue(11).GetType()).ToString().ToUpperInvariant();
                        row.City = reader.GetValue(12) == null ? string.Empty : Convert.ChangeType(reader.GetValue(12), reader.GetValue(12).GetType()).ToString();
                        row.District = reader.GetValue(13) == null ? string.Empty : Convert.ChangeType(reader.GetValue(13), reader.GetValue(13).GetType()).ToString();
                        row.Phone1 = reader.GetValue(14) == null ? string.Empty : Convert.ChangeType(reader.GetValue(14), reader.GetValue(14).GetType()).ToString();
                        row.ShippingName = reader.GetValue(15) == null ? string.Empty : Convert.ChangeType(reader.GetValue(15), reader.GetValue(15).GetType()).ToString().ToUpperInvariant();
                        row.ShippingAddress1 = reader.GetValue(16) == null ? string.Empty : Convert.ChangeType(reader.GetValue(16), reader.GetValue(16).GetType()).ToString();
                        row.CorporateName = reader.GetValue(17) == null ? string.Empty : Convert.ChangeType(reader.GetValue(17), reader.GetValue(17).GetType()).ToString();
                        row.EMailAddress = reader.GetValue(18) == null ? string.Empty : Convert.ChangeType(reader.GetValue(18), reader.GetValue(18).GetType()).ToString();
                        
                        orderList.Add(row);
                    }
                }
            }

            return orderList;
        }
    }
}
