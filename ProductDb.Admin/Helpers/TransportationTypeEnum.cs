using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Helpers
{
    public class TransportationTypeEnum
    {
        public static string product =  "1,3,5";
        public static string brand = "2,4,6";

        public static Dictionary<string, string> GetTip
        {
            get
            {
                var prod = new Dictionary<string, string>();
                prod.Add("Product", product);
                prod.Add("Brand", brand);

                return prod;
            }
        }
    }
}
