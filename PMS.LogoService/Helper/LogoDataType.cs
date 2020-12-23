using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.LogoService.Helper
{
    public enum LogoDataType
    {
        doSalesOrderSlip = 3,
        doAccountsRP = 30,
        doArpShipLic = 34,
        doDateDiffInvoice = 89,
        doSalesInvoice = 19,
        doMaterial = 0,
        doMark = 110
    }

    public enum LogoStatus
    {
        Contain = 1
    }
    public enum LogoResponseCode
    {
        Duplicate = 23000
    }

}
