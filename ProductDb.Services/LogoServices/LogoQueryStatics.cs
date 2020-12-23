using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Services.LogoServices
{
    public class LogoQueryStatics
    {
        static string[] FirmCodes = { "215", "314", "918", "928" };
        private const string LogoStockWarehouseQuery = @"SELECT ROW_NUMBER() OVER(ORDER BY STOK.CODE ASC) AS Id, [FirmCode] =
                                                          (SELECT NR
                                                           FROM      L_CAPIFIRM
                                                           WHERE   NR = {0}), [FirmName] =
                                                          (SELECT NAME
                                                           FROM      L_CAPIFIRM
                                                           WHERE   NR = {0}), STOK.CODE 'Code', STOK.NAME 'Name', AMBAR.NAME 'WarehouseName',AMBAR.NR AS WarehouseNumber, SUM(STINV.ONHAND) - ISNULL
                                                          ((SELECT SUM(AMOUNT - SHIPPEDAMOUNT)
                                                            FROM LG_{0}_{1}_ORFLINE WITH(NOLOCK)
                                                            WHERE STOK.LOGICALREF = STOCKREF AND TRCODE = 1), 0) AS 'Quantity'
                                                            FROM LG_{0}_ITEMS STOK WITH(NOLOCK) LEFT JOIN
                                                            LV_{0}_{1}_STINVTOT STINV WITH(NOLOCK) ON STINV.STOCKREF = STOK.LOGICALREF LEFT JOIN
                                                            L_CAPIWHOUSE AMBAR ON AMBAR.NR = STINV.INVENNO AND AMBAR.FIRMNR = {0}
                                                            WHERE STINV.INVENNO NOT IN (- 1)
                                                            GROUP BY STOK.CODE, STOK.NAME, AMBAR.NAME, STOK.LOGICALREF,AMBAR.NR
                                                            HAVING SUM(STINV.ONHAND) - ISNULL
                                                            ((SELECT SUM(AMOUNT - SHIPPEDAMOUNT)
                                                            FROM LG_{0}_{1}_ORFLINE
                                                             WHERE STOK.LOGICALREF = STOCKREF AND TRCODE = 1), 0) > 0
                                                        ";
        public static string GetLogoStockWarehouseQuery(string PeriodCode = "01")
        {
            StringBuilder query = new StringBuilder();

            int lastIndex = FirmCodes.Length - 1;
            for (int i = 0; i < FirmCodes.Length; i++)
            {
                query.Append(string.Format(LogoStockWarehouseQuery, FirmCodes[i], PeriodCode));
                if (lastIndex != i)
                    query.Append("UNION ALL" + Environment.NewLine);
            }
            return query.ToString();
        }
    }
}
