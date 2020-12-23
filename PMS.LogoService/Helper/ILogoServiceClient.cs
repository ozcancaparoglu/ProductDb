using LogoObjectClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.LogoService.Helper
{
    public interface ILogoServiceClient
    {
        ExecQueryRequest CreateExecuteRequest(string sqlText);
        DirectQueryRequest CreateDirectRequest(string sqlText);
        AppendDataObjectRequest CreateAppendRequest(string dataXml, LogoDataType dataType, int companyId);
    }
}
