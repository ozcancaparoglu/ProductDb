using LogoObjectClient;
using PMS.Common.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PMS.LogoService
{
    public class ReturnedGoodsService
    {
        public const string _g_securitycode = "ocean";
        public const string _LbsLoadPass = "DLGKMEWLKJMRELMREWLK";

        public List<ReturnedItemDto> GetReturnedGoods()
        {
            //Tek sorguya indir
            var SanalMagazaName = GetCompanyName(215);
            var OnnetName = GetCompanyName(314);
            var AristoName = GetCompanyName(104);

            var fromDate = new DateTime(2019, 5, 10);

            return GetReturnedGoods(SanalMagazaName, OnnetName, AristoName, fromDate);
        }

        public string GetCompanyName(int CompanyLogoCode)
        {
            SvcClient LOClient = new SvcClient();

            string sqlText = $"SELECT NAME FROM L_CAPIFIRM WHERE NR={CompanyLogoCode}";

            sqlText = EncodeHelper.Base64Encode(sqlText);

            var req = new ExecQueryRequest()
            {
                errorString = string.Empty,
                LbsLoadPass = _LbsLoadPass,
                orderByText = string.Empty,
                resultXML = string.Empty,
                securityCode = _g_securitycode,
                sqlText = sqlText,
                status = 0
            };

            var result = LOClient.ExecQueryAsync(req).Result;

            if (result.status == 4)
            {
                throw new Exception($"An Error Occured : {result.errorString}");
            }
            else
            {
                string res = StringCompressor.UnzipBase64(result.resultXML.ToString());

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(res.ToString());

                return xml.SelectSingleNode("/RESULTXML/RESULTLINE/NAME").InnerText;
            }
        }

        private List<ReturnedItemDto> GetReturnedGoods(string sanalMagazaName, string onnetName, string aristoName, DateTime fromDate)
        {
            SvcClient LOClient = new SvcClient();

            #region MyRegion

            /*
           Declare @FirmName215 nvarchar(50)
          Declare @FirmName314 nvarchar(50)
          Declare @FirmName104 nvarchar(50)
          Declare @StfcDate nvarchar(50)

          Set @FirmName215 = (SELECT NAME FROM L_CAPIFIRM WHERE NR=215);
          Set @FirmName314 = (SELECT NAME FROM L_CAPIFIRM WHERE NR=314);
          Set @FirmName104 = (SELECT NAME FROM L_CAPIFIRM WHERE NR=104);
          Set @StfcDate = '2019-04-20';
           */

            #endregion MyRegion

            var fromDateStr = $"'{fromDate.Year}-{fromDate.Month}-{fromDate.Day}'";

            #region sqlText

            var sqlText = "SELECT"
                             + $"[CompanyName]='{sanalMagazaName}'"
                             + ",PRJ.CODE 'ProjectCode'"
                             + ",STFC.FICHENO 'MOK'"
                             + ",STFC.DOCODE 'OrderNumber'"
                             + ",STFC.DATE_ 'ReturnDate'"
                             + ",STOK.CODE 'ProductCode'"
                             + ",STOK.NAME 'ProductDesc'"
                             + ",ISNULL(CONVERT(INT,STL.AMOUNT),0) 'ReturnAmount'"
                             + ",STFC.SPECODE 'ReturnReason'"
                             + ",STL.LOGICALREF 'LogoReferans'"
                             + ",STL.DELVRYCODE 'DeliveryCode'"
                             + " FROM "
                             + " LG_215_01_STLINE STL "
                             + " LEFT JOIN LG_215_01_STFICHE STFC ON STFC.LOGICALREF=STL.STFICHEREF "
                             + " LEFT JOIN LG_215_ITEMS STOK ON STOK.LOGICALREF=STL.STOCKREF "
                             + " LEFT JOIN LG_215_PROJECT PRJ ON PRJ.LOGICALREF=STFC.PROJECTREF "
                             + " WHERE "
                             + " STL.TRCODE=3"
                             + " AND STL.LINETYPE=0"
                             + " AND STOK.NAME NOT LIKE '%KARGO BEDELİ%'"
                             + $" AND STFC.DATE_ > {fromDateStr}"
                             + " AND STL.DELVRYCODE = ''"
                             + ""
                             + " UNION ALL"
                             + ""
                             + " SELECT "
                             + $"[CompanyName] = '{onnetName}'"
                             + ",PRJ.CODE 'ProjectCode' "
                             + ",STFC.FICHENO 'MOK'"
                             + ",STFC.DOCODE 'OrderNumber'"
                             + ",stfc.DATE_ 'ReturnDate'"
                             + ",STOK.CODE 'ProductCode'"
                             + ",STOK.NAME 'ProductDesc'"
                             + ",ISNULL(CONVERT(INT,STL.AMOUNT),0) 'ReturnAmount'"
                             + ",STFC.SPECODE 'ReturnReason'"
                             + ",STL.LOGICALREF 'LogoReferans'"
                             + ",STL.DELVRYCODE 'DeliveryCode'"
                             + " FROM "
                             + " LG_314_01_STLINE STL "
                             + " LEFT JOIN LG_314_01_STFICHE STFC ON STFC.LOGICALREF=STL.STFICHEREF "
                             + " LEFT JOIN LG_314_ITEMS STOK ON STOK.LOGICALREF=STL.STOCKREF "
                             + " LEFT JOIN LG_314_PROJECT PRJ ON PRJ.LOGICALREF=STFC.PROJECTREF "
                             + " WHERE "
                             + " STL.TRCODE=3"
                             + " AND STL.LINETYPE=0"
                             + " AND STOK.NAME NOT LIKE '%KARGO BEDELİ%'"
                             + $" AND STFC.DATE_ > {fromDateStr}"
                             + " AND STL.DELVRYCODE = ''"
                             + ""
                             + " UNION ALL"
                             + ""
                             + " SELECT "
                             + $"[CompanyName] = '{aristoName}'"
                             + ",PRJ.CODE 'ProjectCode' "
                             + ",STFC.FICHENO 'MOK'"
                             + ",STFC.DOCODE 'OrderNumber'"
                             + ",stfc.DATE_ 'ReturnDate'"
                             + ",STOK.CODE 'ProductCode'"
                             + ",STOK.NAME 'ProductDesc'"
                             + ",ISNULL(CONVERT(INT,STL.AMOUNT),0) 'ReturnAmount'"
                             + ",STFC.SPECODE 'ReturnReason'"
                             + ",STL.LOGICALREF 'LogoReferans'"
                             + ",STL.DELVRYCODE 'DeliveryCode'"
                             + " FROM "
                             + " LG_104_01_STLINE STL "
                             + " LEFT JOIN LG_104_01_STFICHE STFC ON STFC.LOGICALREF=STL.STFICHEREF "
                             + " LEFT JOIN LG_104_ITEMS STOK ON STOK.LOGICALREF=STL.STOCKREF "
                             + " LEFT JOIN LG_104_PROJECT PRJ ON PRJ.LOGICALREF=STFC.PROJECTREF "
                             + " WHERE "
                             + " STL.TRCODE=3"
                             + " AND STL.LINETYPE=0"
                             + " AND STOK.NAME NOT LIKE '%KARGO BEDELİ%'"
                             + $"AND STFC.DATE_ > {fromDateStr}"
                             + " AND STL.DELVRYCODE = ''";
            //" @StfcDate";

            #endregion sqlText
            
            var req = new ExecQueryRequest()
            {
                errorString = string.Empty,
                LbsLoadPass = _LbsLoadPass,
                orderByText = "order by ReturnDate",
                resultXML = string.Empty,
                securityCode = _g_securitycode,
                sqlText = EncodeHelper.Base64Encode(sqlText),
                status = 0
            };

            var result = LOClient.ExecQueryAsync(req).Result;

            if (result.status == 4)
            {
                throw new Exception($"An Error Occured : {result.errorString}");
            }
            else
            {
                string res = StringCompressor.UnzipBase64(result.resultXML.ToString());

                XDocument xdoc = new XDocument();
                xdoc = XDocument.Parse(res);

                var resultLines = xdoc.Element("RESULTXML").Elements("RESULTLINE");

                var ReturnedGoodsList = new List<ReturnedItemDto>();

                ReturnedGoodsList =
                    resultLines.Select(x => new ReturnedItemDto()
                    {
                        CompanyName = NullController(x.Element("CompanyName")),
                        MOK = NullController(x.Element("MOK")),
                        OrderNumber = NullController(x.Element("OrderNumber")),
                        ProductCode = NullController(x.Element("ProductCode")),
                        ProductDesc = NullController(x.Element("ProductDesc")),
                        ProjectCode = NullController(x.Element("ProjectCode")),
                        ReturnAmount = Convert.ToInt32(string.IsNullOrWhiteSpace(NullController(x.Element("ReturnAmount"))) ? "0" : NullController(x.Element("ReturnAmount"))),
                        ReturnDateStr = DateTimeController(x.Element("ReturnDate")),
                        ReturnReason = NullController(x.Element("ReturnReason")),
                        LogoReferans = NullController(x.Element("LogoReferans")),
                        DeliveryCode = NullController(x.Element("DeliveryCode"))
                    }).ToList();

                return ReturnedGoodsList;
            }
        }

        private string DateTimeController(XElement element)
        {
            return (element != null && element.IsEmpty == false) ?
                Convert.ToDateTime(element.Value).ToString("dd'/'MM'/'yyyy")
                :
                string.Empty;
        }

        private static string NullController(XElement element)
        {
            return (element != null && element.IsEmpty == false) ? element.Value : string.Empty;
        }


        public bool FlagReturnedItemAsProcessed(int logicalRef)
        {
            return true;

            //SvcClient LOClient = new SvcClient();

            //string sqlText = $"UPDATE LG_215_01_STLINE SET DELVRYCODE = 'TTGOK' WHERE TRCODE = 3 AND LOGICALREF = {logicalRef}";

            //sqlText = EncodeHelper.Base64Encode(sqlText);

            //var req = new ExecQueryRequest()
            //{
            //    errorString = string.Empty,
            //    LbsLoadPass = _LbsLoadPass,
            //    orderByText = string.Empty,
            //    resultXML = string.Empty,
            //    securityCode = _g_securitycode,
            //    sqlText = sqlText,
            //    status = 0
            //};

            //var result = LOClient.ExecQueryAsync(req).Result;

            //if (result.status == 4)
            //{
            //    return false;
            //    //throw new Exception($"An Error Occured : {result.errorString}");
            //}
            //else
            //{
            //    return true;
            //}
        }
        
        public async Task GetShiteAsync()
        {
            SvcClient LOClient = new SvcClient();

            string something = "";

            string sqlText = "SELECT NAME FROM L_CAPIFIRM WHERE NR=215";

            /*
             Declare @FirmName215 nvarchar(50)
Declare @FirmName314 nvarchar(50)
Declare @FirmName104 nvarchar(50)
Declare @StfcDate nvarchar(50)

Set @FirmName215 = (SELECT NAME FROM L_CAPIFIRM WHERE NR=215);
Set @FirmName314 = (SELECT NAME FROM L_CAPIFIRM WHERE NR=314);
Set @FirmName104 = (SELECT NAME FROM L_CAPIFIRM WHERE NR=104);
Set @StfcDate = '2019-04-20';

SELECT
	[CompanyName]=@FirmName215
	,PRJ.CODE 'ProjectCode'
	,STFC.FICHENO 'MOK'
	,STFC.DOCODE 'OrderNumber'
	,STFC.DATE_ 'ReturnDate'
	,STOK.CODE 'ProductCode'
	,STOK.NAME 'ProductDesc'
	,STL.AMOUNT 'ReturnAmount'
	,STFC.SPECODE 'ReturnReason'
FROM
	LG_215_01_STLINE STL
	LEFT JOIN LG_215_01_STFICHE STFC ON STFC.LOGICALREF=STL.STFICHEREF
	LEFT JOIN LG_215_ITEMS STOK ON STOK.LOGICALREF=STL.STOCKREF
	LEFT JOIN LG_215_PROJECT PRJ ON PRJ.LOGICALREF=STFC.PROJECTREF
WHERE
	STL.TRCODE=3
	AND STL.LINETYPE=0
	AND STOK.NAME NOT LIKE '%KARGO BEDELİ%'
	AND STFC.DATE_ > @StfcDate

UNION ALL

SELECT
	[CompanyName] = @FirmName314
	,PRJ.CODE 'ProjectCode'
	,STFC.FICHENO 'MOK'
	,STFC.DOCODE 'OrderNumber'
	,stfc.DATE_ 'ReturnDate'
	,STOK.CODE 'ProductCode'
	,STOK.NAME 'ProductDesc'
	,STL.AMOUNT 'ReturnAmount'
	,STFC.SPECODE 'ReturnReason'
FROM
	LG_314_01_STLINE STL
	LEFT JOIN LG_314_01_STFICHE STFC ON STFC.LOGICALREF=STL.STFICHEREF
	LEFT JOIN LG_314_ITEMS STOK ON STOK.LOGICALREF=STL.STOCKREF
	LEFT JOIN LG_314_PROJECT PRJ ON PRJ.LOGICALREF=STFC.PROJECTREF
WHERE
	STL.TRCODE=3
	AND STL.LINETYPE=0
	AND STOK.NAME NOT LIKE '%KARGO BEDELİ%'
	AND STFC.DATE_ > @StfcDate

UNION ALL

SELECT
	[CompanyName]=@FirmName104
	,PRJ.CODE 'ProjectCode'
	,STFC.FICHENO 'MOK'
	,STFC.DOCODE 'OrderNumber'
	,stfc.DATE_ 'ReturnDate'
	,STOK.CODE 'ProductCode'
	,STOK.NAME 'ProductDesc'
	,STL.AMOUNT 'ReturnAmount'
	,STFC.SPECODE 'ReturnReason'
FROM
	LG_104_01_STLINE STL
	LEFT JOIN LG_104_01_STFICHE STFC ON STFC.LOGICALREF=STL.STFICHEREF
	LEFT JOIN LG_104_ITEMS STOK ON STOK.LOGICALREF=STL.STOCKREF
	LEFT JOIN LG_104_PROJECT PRJ ON PRJ.LOGICALREF=STFC.PROJECTREF
WHERE
	STL.TRCODE=3
	AND STL.LINETYPE=0
	AND STOK.NAME NOT LIKE '%KARGO BEDELİ%'
	AND STFC.DATE_ > @StfcDate
             */

            object resultXML = "";
            string errorString = "";
            byte status = 0;
            //string securityCode = "a5020207-3a24-437f-951";

            var req = new ExecQueryRequest()
            {
                errorString = string.Empty,
                securityCode = _g_securitycode,
                LbsLoadPass = "DLGKMEWLKJMRELMREWLK",
                orderByText = "ORDER BY ReturnDate DESC",
                resultXML = string.Empty,
                sqlText = EncodeHelper.Base64Encode(sqlText),
                status = 0
            };

            Task<ExecQueryResponse> resultTask = LOClient.ExecQueryAsync(req);
            ExecQueryResponse result = await resultTask;
            //(sqlText, "ORDER BY LOGICALREF", securityCode, ref resultXML, ref errorString, ref status, "");

            if (status == 4)
                something = (errorString + "\r");
            else
            {
                int n = GetObjectSize(resultXML);
                object res = StringCompressor.UnzipBase64(resultXML.ToString());
                int m = GetObjectSize(res);

                //               ResultTextBox.AppendText("Gelen Sıkıştırılmış Veri Miktarı : "
                //+ Convert.ToInt32(n) + " Bayt\r");
                //               ResultTextBox.AppendText("Açılmış Veri Miktarı : "
                //+ Convert.ToInt32(m) + " Bayt\r");
                //               ResultTextBox.AppendText("\r");
                //               ResultTextBox.Refresh();

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(res.ToString());

                XmlNodeList xnList = xml.SelectNodes("/RESULTXML/RESULTLINE");
                foreach (XmlNode xn in xnList)
                {
                    string code = xn["CODE"].InnerText;
                    string definition = xn["DEFINITION_"].InnerText;

                    something = (code + " " + definition + "\r");
                }
            }
        }

        private int GetObjectSize(object TestObject)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            byte[] Array;
            bf.Serialize(ms, TestObject);
            Array = ms.ToArray();
            return Array.Length;
        }
    }
}