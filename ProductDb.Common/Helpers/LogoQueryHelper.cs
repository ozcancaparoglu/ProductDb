using ProductDb.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.Helpers
{
    public class LogoQueryHelper
    {
        public LogoQueryHelper()
        {

        }
        //public static string GetQueryByWareHouse(Logowarehouse logowarehouse)
        //{

        //    var TablePrefix = GetTablePrefixByWareHouse(logowarehouse);

        //    string query = string.Format("{0}{1}", "SELECT " +
        //      "ISNULL((ROW_NUMBER() OVER(ORDER BY STOK ASC)), 0) AS 'Id', " +
        //      "MLZ_KOD, MLZ_ACIKLAMA, AMBAR_ID, STOK, AMBAR_ADI " +
        //      "FROM URUN_WEB_STOK", TablePrefix);

        //    return query;
        //}

    }
}
