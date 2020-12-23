
using LogoObjectClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.LogoService.Helper
{
    public class LogoServiceClient: ILogoServiceClient
    {
        #region Fields
        private const string paramXML =
               "<?xml version=\"1.0\" encoding=\"utf-16\"?>"
               + "<Parameters>"
               + "  <ReplicMode>0</ReplicMode>"
               + "  <CheckParams>1</CheckParams>"
               + "  <CheckRight>1</CheckRight>"
               + "  <ApplyCampaign>0</ApplyCampaign>"
               + "  <ApplyCondition>0</ApplyCondition>"
               + "  <FillAccCodes>0</FillAccCodes>"
               + "  <FormSeriLotLines>0</FormSeriLotLines>"
               + "  <GetStockLinePrice>0</GetStockLinePrice>"
               + "  <ExportAllData>0</ExportAllData>"
               + "</Parameters>";

        private readonly IConfiguration configuration; 
        #endregion

        public LogoServiceClient(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private static SvcClient Client = null;
        public static SvcClient SvcClient
        {
            get
            {
                if (Client == null)
                    Client = new SvcClient();
                return Client;
            }
        }

        public ExecQueryRequest CreateExecuteRequest(string sqlText)
        {
            var req = new ExecQueryRequest()
            {
                errorString = string.Empty,
                LbsLoadPass = configuration.GetSection("LogoServiceCredentials:_LbsLoadPass").Value,
                orderByText = string.Empty,
                resultXML = string.Empty,
                securityCode = configuration.GetSection("LogoServiceCredentials:_g_securitycode").Value,
                sqlText = EncodeHelper.Base64Encode(sqlText),
                status = 0,
            };

            return req;
        }

        public  AppendDataObjectRequest CreateAppendRequest(string dataXml, LogoDataType dataType, int companyId)
        {
            var req = new AppendDataObjectRequest()
            {
                dataType = (int)dataType,
                errorString = string.Empty,
                LbsLoadPass = configuration.GetSection("LogoServiceCredentials:_LbsLoadPass").Value,
                securityCode = configuration.GetSection("LogoServiceCredentials:_g_securitycode").Value,
                paramXML = paramXML,
                dataXML = StringCompressor.ZipBase64(dataXml),
                dataReference = 0,
                status = 0,
                FirmNr = companyId
            };

            return req;
        }

        public DirectQueryRequest CreateDirectRequest(string sqlText)
        {
            var req = new DirectQueryRequest()
            {
                sqlText = EncodeHelper.Base64Encode(sqlText),
                errorString = string.Empty,
                LbsLoadPass = configuration.GetSection("LogoServiceCredentials:_LbsLoadPass").Value,
                securityCode = configuration.GetSection("LogoServiceCredentials:_g_securitycode").Value,
                status = 0
            };

            return req;
        }
    }
}
