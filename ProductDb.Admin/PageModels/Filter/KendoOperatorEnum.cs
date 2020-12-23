using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.Filter
{
    public enum KendoOperatorEnum
    {
        isequalto = 0,
        isnotequalto = 1,
        startswith = 2,
        contains = 3,
        doesnotcontain = 4,
        endswith = 5,
        isnull = 6,
        isnotnull = 7,
        isempty = 8,
        isnotempty = 9,
        hasnovalue = 10,
        hasvalue = 11
    }
}
