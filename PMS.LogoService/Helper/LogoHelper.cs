using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PMS.LogoService.Helper
{
    public class LogoHelper
    {
        private static string[] SuccessErrorKeys = { "23000" };

        private const string DateDiffInvoiceQuery = @"SELECT PRJ.CODE 
                                                    'PROJECTCODE',ORFC.FICHENO 'ORDERNO' ,
                                                     INV.FICHENO 'INVOICENO' , SEVK.CODE  'SHIPPINGADRESSCODE', SEVK.NAME 'SHIPPINGNAME' ,
                                                     SEVK.ADDR1 'ADRESS1' , SEVK.ADDR2 'ADRESS2',SEVK.TOWN 'TOWN',SEVK.CITY 'CITY' ,
                                                     SEVK.DISTRICT 'DISTRICT' ,SEVK.TELNRS1 'TELNRS1',SEVK.TELNRS2 'TELNRS2',SEVK.TAXOFFICE 'TAXOFFICE'  
                                                    ,SEVK.TAXNR 'TAXNR',CONVERT(varchar(20), TALAN.NUMFLDS2) as 'PRICE',CONVERT(varchar(20), orfl.VAT)  as 'VATRATE',ORFc.DELIVERYCODE,
                                                     ITM.NAME AS ITEMNAME,
                                                     ITM.CODE AS ITEMCODE,
                                                     SEVK.EMAILADDR,
													 SEVK.COUNTRY,
													 SEVK.COUNTRYCODE,
                                                     INV.DATE_ AS INVOICEDATE,
                                                     INV.GENEXCTYP,
													 INV.LINEEXCTYP,
                                                     ORFL.LOGICALREF AS LINELOGICALREF
                                                     FROM LG_{0}_{1}_ORFLINE ORFL LEFT JOIN 
                                                     LG_{0}_{1}_ORFICHE ORFC ON ORFC.LOGICALREF=ORFL.ORDFICHEREF LEFT JOIN
                                                     LG_{0}_SHIPINFO SEVK ON SEVK.LOGICALREF=ORFC.SHIPINFOREF LEFT JOIN
                                                     LG_{0}_{1}_DEFNFLDSTRANV TALAN ON TALAN.OWNERREF=ORFL.LOGICALREF AND TALAN.MODULENR=8 LEFT JOIN
                                                     LG_{0}_{1}_STLINE STL ON STL.ORDTRANSREF=ORFL.LOGICALREF LEFT JOIN
                                                     LG_{0}_{1}_INVOICE INV ON INV.LOGICALREF=STL.INVOICEREF LEFT JOIN
                                                     LG_{0}_PROJECT PRJ ON PRJ.LOGICALREF=ORFL.PROJECTREF LEFT JOIN
                                                     LG_{0}_ITEMS ITM ON ITM.LOGICALREF = STL.STOCKREF
                                                     WHERE  ORFC.DATE_> '2019-11-01'  AND TALAN.NUMFLDS2 > 0 AND INV.FICHENO IS NOT NULL
                                                     AND ORFL.DELVRYCODE <> '3'
                                                    ";

        private const string CheckingAccountControlQuery = @"SELECT COUNT(CODE) as CODE FROM LG_{0}_CLCARD
                                                             WHERE CODE = '{1}'";

        private const string ORFICHE = @"SELECT * FROM LG_{0}_{1}_ORFICHE
                                                             WHERE FICHENO = '{2}'";

        private const string ITEMSQUERY = @"SELECT CODE FROM LG_{0}_ITEMS
                                                             WHERE CODE = '{1}'";
        private const string CapiDeptQuery = @"SELECT [LOGICALREF]
                                      ,[FIRMNR]
                                      ,[NR]
                                      ,[NAME]
                                  FROM [SANALMAGAZA].[dbo].[L_CAPIDEPT] WHERE FIRMNR = {0}";

        private const string CapiDivQuery = @"SELECT [LOGICALREF],[FIRMNR],[NR],[NAME]
                                            FROM [SANALMAGAZA].[dbo].[L_CAPIDIV] where FIRMNR = {0}";

        private const string CapiFirmQuery = @"SELECT [LOGICALREF],[NR],[NAME],[TITLE]
                                            FROM [SANALMAGAZA].[dbo].[L_CAPIFIRM]";

        private const string UpdateORFCDeliveryCode = @"UPDATE LG_{0}_{1}_ORFLINE SET DELVRYCODE = 3
                                                             WHERE LOGICALREF = '{2}'";

        private const string TrackingNumberQuery = @"SELECT  ORFC.DOCODE 'OrderNo',STOK.CODE 'Code',TALAN.TEXTFLDS5 'TrackingNumber'
                                                    ,TALAN.TEXTFLDS30 'CargoFirm'
                                                    FROM LG_{0}_{1}_ORFLINE  ORFL WITH(NOLOCK) LEFT JOIN
                                                    LG_{0}_{1}_ORFICHE ORFC WITH(NOLOCK) ON ORFC.LOGICALREF=ORFL.ORDFICHEREF LEFT JOIN
                                                    LG_{0}_{1}_STLINE STL WITH(NOLOCK) ON STL.ORDTRANSREF=ORFL.LOGICALREF LEFT JOIN
                                                    LG_{0}_{1}_STFICHE STFC WITH(NOLOCK) ON STFC.LOGICALREF=STL.STFICHEREF LEFT JOIN
                                                    LG_{0}_{1}_DEFNFLDSTRANV TALAN WITH(NOLOCK) ON TALAN.PARENTREF=STFC.LOGICALREF AND MODULENR=9 AND OWNERREF=0 LEFT JOIN       
                                                    LG_{0}_ITEMS STOK WITH(NOLOCK) ON STOK.LOGICALREF=ORFL.STOCKREF LEFT JOIN
                                                    LG_{0}_PROJECT PRJ WITH(NOLOCK) ON PRJ.LOGICALREF=ORFL.PROJECTREF LEFT JOIN
                                                    LG_{0}_CLCARD CARI WITH(NOLOCK) ON CARI.LOGICALREF=ORFL.CLIENTREF LEFT JOIN
                                                    LG_{0}_SHIPINFO SEVK WITH(NOLOCK) ON ORFC.SHIPINFOREF=SEVK.LOGICALREF
                                                    WHERE ORFL.LINETYPE=0 AND ORFC.TRCODE=1 
					                                  AND STOK.CODE NOT LIKE 'ZKRGBDL' 
					                                  AND STFC.FICHENO IS NOT NULL --MOK_NO
					                                  AND TALAN.TEXTFLDS5 IS NOT NULL AND TALAN.TEXTFLDS5<>''   --KARGO_TAKIP_NO
					                                  AND ORFC.DOCODE IN({2})";

        public const string GL_CODE = "120.11.D001";

        private static Dictionary<string, string> _countries = null;

        public static Dictionary<string, string> Countries
        {
            get
            {
                if (_countries == null)
                    _countries = GetCountries();

                return _countries;
            }
        }

        public static Dictionary<string, string> GetCountries()
        {
            Dictionary<string, string> newDic = new Dictionary<string, string>();

            newDic.Add("TÜRKİYE", "TR");
            //---
            newDic.Add("A.B.D.", "US");
            newDic.Add("AFGANİSTAN", "AF");
            newDic.Add("ALMANYA", "DE");
            newDic.Add("AMERİKAKÜÇÜKOUT.ADALARI", "UM");
            newDic.Add("AMERİKANSAMOA", "AS");
            newDic.Add("AMERİKANVİRGİNADALARI", "");
            newDic.Add("ANDORRA", "AD");
            newDic.Add("ANGOLA", "AO");
            newDic.Add("ANGUİLLA", "AI");
            newDic.Add("ANTARTİKA", "AQ");
            newDic.Add("ANTİGUA-BARBUDA", "AG");
            newDic.Add("ARJANTİN", "AR");
            newDic.Add("ARNAVUTLUK", "AL");
            newDic.Add("ARUBA", "AW");
            newDic.Add("AVUSTRALYA", "AU");
            newDic.Add("AVUSTURYA", "AT");
            newDic.Add("AZERBAYCAN-NAHÇ.", "AZ");
            newDic.Add("BAHAMALAR", "BS");
            newDic.Add("BAHREYN", "BH");
            newDic.Add("BANGLADEŞ", "BD");
            newDic.Add("BARBADOS", "BB");
            newDic.Add("BATITİMOR", "TL");
            newDic.Add("BELÇİKA", "BE");
            newDic.Add("BELİZE", "BZ");
            newDic.Add("BENİN", "BJ");
            newDic.Add("BERMUDA", "BM");
            newDic.Add("BEYAZRUSYA", "BY");
            newDic.Add("BHUTAN", "BT");
            newDic.Add("BİRLEŞİKARAPEMİRLİKLERİ", "AE");
            newDic.Add("BOLİVYA", "BO");
            newDic.Add("BOSNAHERSEK", "BA");
            newDic.Add("BOTSVANA", "BW");
            newDic.Add("BOUVETADASI", "BV");
            newDic.Add("BREZİLYA", "BR");
            newDic.Add("BRUNEİ", "BN");
            newDic.Add("BULGARİSTAN", "BG");
            newDic.Add("BURKİNAFASO", "BF");
            newDic.Add("BURMA(BİRMANYA)", "MM");
            newDic.Add("BURUNDİ", "BI");
            newDic.Add("CAPEVERDE", "CV");
            newDic.Add("CAYMANADALARI", "KY");
            newDic.Add("CEBELİTARIK", "GI");
            newDic.Add("CEUTA", "XC");
            newDic.Add("CEZAYİR", "DZ");
            newDic.Add("CİBUTİ", "DJ");
            newDic.Add("COCOSADALARI", "CC");
            newDic.Add("COOKADASI", "CK");
            newDic.Add("ÇAD", "TD");
            newDic.Add("ÇEKCUMHURİYETİ", "CZ");
            newDic.Add("ÇİNHALKCUMHUR.", "CN");
            newDic.Add("DANİMARKA", "DK");
            newDic.Add("DOMİNİKCUMHUR.", "DO");
            newDic.Add("DOMİNİKA", "DM");
            newDic.Add("EKVATOR", "EC");
            newDic.Add("EKVATORGİNESİ", "GQ");
            newDic.Add("ELSALVADOR", "SV");
            newDic.Add("ENDONEZYA", "ID");
            newDic.Add("ERİTRE", "ER");
            newDic.Add("ERMENİSTAN", "AM");
            newDic.Add("ESTONYA", "EE");
            newDic.Add("ETİYOPYA", "ET");
            newDic.Add("FALKLANDADALARI", "FK");
            newDic.Add("FAROEADALARI", "FO");
            newDic.Add("FAS", "MA");
            newDic.Add("FİJİ", "FJ");
            newDic.Add("FİLDİŞİSAHİLİ", "CI");
            newDic.Add("FİLİPİNLER", "PH");
            newDic.Add("FİNLANDİYA", "FI");
            newDic.Add("FRANSA", "FR");
            newDic.Add("FRANSAGÜNEYBÖLGELERİ", "TF");
            newDic.Add("FRANSIZPOLİNEZYASI", "PF");
            newDic.Add("GABON", "GA");
            newDic.Add("GAMBİYA", "GM");
            newDic.Add("GANA", "GH");
            newDic.Add("GİNE", "GN");
            newDic.Add("GİNE-BİSSAU", "GW");
            newDic.Add("GRENADA", "GD");
            newDic.Add("GRÖNLAND", "GL");
            newDic.Add("GUAM", "GU");
            newDic.Add("GUATEMALA", "GT");
            newDic.Add("GUYANA", "GY");
            newDic.Add("GÜNEYAFRİKACUM.", "ZA");
            newDic.Add("GÜNEYGEORGİAVEGÜNEYSANDVİÇADALARI", "GS");
            newDic.Add("GÜNEYKORECUM.", "KR");
            newDic.Add("GÜRCİSTAN", "GE");
            newDic.Add("HAİTİ", "HT");
            newDic.Add("HEARDADALAIVEMCDONALDADASI", "HM");
            newDic.Add("HIRVATİSTAN", "HR");
            newDic.Add("HİNDİSTAN", "IN");
            newDic.Add("HOLLANDA", "NL");
            newDic.Add("HOLLANDAANTİLLERİ", "AN");
            newDic.Add("HONDURAS", "HN");
            newDic.Add("HONG-KONG", "HK");
            newDic.Add("IRAK", "IQ");
            newDic.Add("İNGİLİZHİNTOKYANUSUTOPRAKLARI", "IO");
            newDic.Add("İNGİLİZVİRGİNADALARI", "VG");
            newDic.Add("İNGİLTERE", "GB");
            newDic.Add("İRAN", "IR");
            newDic.Add("İRLANDA", "IE");
            newDic.Add("İSPANYA", "ES");
            newDic.Add("İSRAİL", "IL");
            newDic.Add("İSVEÇ", "SE");
            newDic.Add("İSVİÇRE", "CH");
            newDic.Add("İŞGALALTINDAKİFİLİSTİNTOPRAKLARI", "PS");
            newDic.Add("İTALYA", "IT");
            newDic.Add("İZLANDA", "IS");
            newDic.Add("JAMAİKA", "JM");
            newDic.Add("JAPONYA", "JP");
            newDic.Add("KAMBOÇYA", "KH");
            newDic.Add("KAMERUN", "CM");
            newDic.Add("KANADA", "CA");
            newDic.Add("KARADAĞ", "ME");
            newDic.Add("KATAR", "QA");
            newDic.Add("KAZAKİSTAN", "KZ");
            newDic.Add("KENYA", "KE");
            newDic.Add("KIBRISRUMKES.", "CY");
            newDic.Add("KIRGIZİSTAN", "KG");
            newDic.Add("KİRİBATİ", "KI");
            newDic.Add("KOLOMBİYA", "CO");
            newDic.Add("KOMORLAR", "KM");
            newDic.Add("KONGO", "CG");
            newDic.Add("KONGODEM.CUM", "CD");
            newDic.Add("KOSOVA", "XK");
            newDic.Add("KOSTARİKA", "CR");
            newDic.Add("KRİSMISADALARI", "CX");
            newDic.Add("KUVEYT", "KW");
            newDic.Add("KUZEYKORE", "KP");
            newDic.Add("KUZEYMARİANAADALARI", "MP");
            newDic.Add("KÜBA", "CU");
            newDic.Add("LAOSDEMOK.HALKC", "LA");
            newDic.Add("LESOTO", "LS");
            newDic.Add("LETONYA", "LV");
            newDic.Add("LIECHTENSTEIN", "LI");
            newDic.Add("LİBERYA", "LR");
            newDic.Add("LİBYA", "LY");
            newDic.Add("LİTVANYA", "LT");
            newDic.Add("LÜBNAN", "LB");
            newDic.Add("LÜKSEMBURG", "LU");
            newDic.Add("MACARİSTAN", "HU");
            newDic.Add("MADAGASKAR", "MG");
            newDic.Add("MAKAO", "MO");
            newDic.Add("MAKEDONYA", "MK");
            newDic.Add("MALAVİ", "MW");
            newDic.Add("MALDİVLER", "MV");
            newDic.Add("MALEZYA", "MY");
            newDic.Add("MALİ", "ML");
            newDic.Add("MALTA", "MT");
            newDic.Add("MARSHALADALARI", "MH");
            newDic.Add("MARTİNİK", "");
            newDic.Add("MAURİTİUS", "MU");
            newDic.Add("MAYOTTE", "YT");
            newDic.Add("MEKSİKA", "MX");
            newDic.Add("MELİLLA", "XL");
            newDic.Add("MISIR", "EG");
            newDic.Add("MİKRONEZYA", "FM");
            newDic.Add("MOĞOLİSTAN", "MN");
            newDic.Add("MOLDOVYA", "MD");
            newDic.Add("MONTSERRAT", "MS");
            newDic.Add("MORİTANYA", "MR");
            newDic.Add("MOZAMBİK", "MZ");
            newDic.Add("NAMİBYA", "NA");
            newDic.Add("NAURU", "NR");
            newDic.Add("NEPAL", "NP");
            newDic.Add("NIUEADASI", "NU");
            newDic.Add("NİJER", "NE");
            newDic.Add("NİJERYA", "NG");
            newDic.Add("NİKARAGUA", "NI");
            newDic.Add("NORFOLKADASI", "NF");
            newDic.Add("NORVEÇ", "NO");
            newDic.Add("ORTAAFRİKACUM", "CF");
            newDic.Add("ÖZBEKİSTAN", "UZ");
            newDic.Add("PAKİSTAN", "PK");
            newDic.Add("PALAU", "PW");
            newDic.Add("PANAMA", "PA");
            newDic.Add("PAPUA(YENİGİNE)", "PG");
            newDic.Add("PARAGUAY", "PY");
            newDic.Add("PERU", "PE");
            newDic.Add("PİTCAİRN", "PN");
            newDic.Add("POLONYA", "PL");
            newDic.Add("PORTEKİZ", "PT");
            newDic.Add("ROMANYA", "RO");
            newDic.Add("RUANDA", "RW");
            newDic.Add("RUSYAFEDERASYONU", "RU");
            newDic.Add("SAMOA", "WS");
            newDic.Add("SANMARİNO", "SM");
            newDic.Add("SAOTOMEVEPRIN.", "ST");
            newDic.Add("SENEGAL", "SN");
            newDic.Add("SEYŞELLER", "SC");
            newDic.Add("SIRBİSTAN", "XS");
            newDic.Add("SİERRALEONE", "SL");
            newDic.Add("SİNGAPUR", "SG");
            newDic.Add("SLOVAKCUMHURİYETİ", "SK");
            newDic.Add("SLOVENYA", "SI");
            newDic.Add("SOLOMONADALARI", "SB");
            newDic.Add("SOMALİ", "SO");
            newDic.Add("SRİLANKA", "LK");
            newDic.Add("ST.HELENA", "SH");
            newDic.Add("ST.KITTSVENEVİS", "KN");
            newDic.Add("ST.PIERRE&MIQUELON", "PM");
            newDic.Add("ST.LUCİA", "LC");
            newDic.Add("ST.VİNCENT&GRENADİNES", "");
            newDic.Add("SUDAN", "SD");
            newDic.Add("SURİNAM", "SR");
            newDic.Add("SURİYEARAPCUMHURİYETİ", "SY");
            newDic.Add("SUUDİARABİSTAN", "SA");
            newDic.Add("SVAZİLAND", "SZ");
            newDic.Add("ŞİLİ", "CL");
            newDic.Add("TACİKİSTAN", "TJ");
            newDic.Add("TANZANYA", "TZ");
            newDic.Add("TAYLAND", "TH");
            newDic.Add("TAYVAN", "TW");
            newDic.Add("TOGO", "TG");
            newDic.Add("TOKELAU", "TK");
            newDic.Add("TONGA", "TO");
            newDic.Add("TRİNİDADVETOBAGO", "TT");
            newDic.Add("TUNUS", "TN");
            newDic.Add("TURKSVECAİCAOSADALARI", "TC");
            newDic.Add("TUVALU", "TV");
            newDic.Add("TÜRKMENİSTAN", "TM");
            newDic.Add("UGANDA", "UG");
            newDic.Add("UKRAYNA", "UA");
            newDic.Add("UMMAN", "OM");
            newDic.Add("URUGUAY", "UY");
            newDic.Add("ÜRDÜN", "JO");
            newDic.Add("VANUATU", "VU");
            newDic.Add("VATİKAN", "");
            newDic.Add("VENEZUELLA", "VE");
            newDic.Add("VİETNAMSOSYALİST", "VN");
            newDic.Add("WALLİSVEFUTUNA", "WF");
            newDic.Add("YEMEN", "YE");
            newDic.Add("YENİKALEDONYA", "NC");
            newDic.Add("YENİZELANDA", "NZ");
            newDic.Add("YENİZELANDAOKY.", "");
            newDic.Add("YUNANİSTAN", "GR");
            newDic.Add("ZAMBİYA", "ZM");
            newDic.Add("ZİMBABVE", "ZW");

            return newDic;
        }
        public static string GetCountryCode(string CountryCode)
        {
            var value = "";
            var strReturn = true;
            strReturn = Countries.Select(x => x.Key).Contains(CountryCode);
            if (!strReturn)
            {
                strReturn = Countries.Select(x => x.Value).Contains(CountryCode);
                if (strReturn)
                    value = Countries.Where(s => s.Value == CountryCode).FirstOrDefault().Value;
            }
            else
            {
                value = Countries.Where(s => s.Key == CountryCode).FirstOrDefault().Value;
            }

            if (string.IsNullOrWhiteSpace(value))
                value = "TR";

            return value;
        }


        public static string GetCapiFirmQuery()
        {
            var _CapiFirmQuery = CapiFirmQuery;
            return _CapiFirmQuery;
        }

        public static string GetDateDiffInvoiceQuery(int companyid, string periodid)
        {
            var _dateDiffInvoiceQuery = string.Format(DateDiffInvoiceQuery,
                                        companyid.ToString(), periodid);
            return _dateDiffInvoiceQuery;
        }
        public static string GetCheckingAccountQuery(int companyid, string code)
        {
            var _checkingAccount = string.Format(CheckingAccountControlQuery,
                                        companyid.ToString(), code);
            return _checkingAccount;
        }
        public static string GetCapiDeptQuery(int companyid)
        {
            var _checkingAccount = string.Format(CapiDeptQuery,
                                        companyid.ToString());
            return _checkingAccount;
        }

        public static string GetCapiDivQuery(int companyid)
        {
            var _checkingAccount = string.Format(CapiDivQuery,
                                        companyid.ToString());
            return _checkingAccount;
        }

        public static string GetOrderTrackingNumberQuery(int companyid, string periodid,string orderNumbers)
        {
            var _trackingNumberQuery = string.Format(TrackingNumberQuery,
                                        companyid.ToString(), periodid, orderNumbers);
            return _trackingNumberQuery;
        }


        public static string GetUpdateORFCDeliveryCodeQuery(int companyid, string periodid, string logicalRef)
        {
            var _updateORFCDeliveryCodeQuery = string.Format(UpdateORFCDeliveryCode,
                                        companyid.ToString(), periodid, logicalRef);
            return _updateORFCDeliveryCodeQuery;
        }
        public static string GetORFICHEQuery(int companyid, string periodid, string FICHENO)
        {
            var _updateORFCDeliveryCodeQuery = string.Format(ORFICHE,
                                        companyid.ToString(), periodid, FICHENO);

            return _updateORFCDeliveryCodeQuery;
        }

        public static string[] LogoDateFormatter(DateTime Date)
        {
            var formatted = string.Format("{0:dd.MM.yyyy HH:mm:ss}", Date);

            var splittedDate = formatted.Split(" ");
            // shortdate
            var shortDate = splittedDate[0];
            // dateDetail
            var dateDetail = splittedDate[1].Split(":");
            // details
            var hour = dateDetail[0];
            var minute = dateDetail[1];
            var second = dateDetail[2];
            var milisecond = "0";

            return new string[]
            {
                shortDate,
                hour,
                minute,
                second,
                milisecond
            };
        }
        public static decimal FormattedPrice(string price)
        {
            if (CultureInfo.CurrentCulture.Name == "tr-TR")
                return Convert.ToDecimal(price.Replace(".", ","));

            return Convert.ToDecimal(price);
        }

        public static string ItemControl(int companyid, string code)
        {
            var _checkingItem = string.Format(ITEMSQUERY,
                                        companyid.ToString(), code);
            return _checkingItem;
        }
        public static bool LogoResponseStatus(string message)
        {
            bool status = false;
            // Success Error Keys
            foreach (var item in SuccessErrorKeys)
            {
                status = message.Contains(item);
                if (status)
                    break;
            }
            return status;
        }
    }


}

