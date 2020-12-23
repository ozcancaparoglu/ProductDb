using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PMS.LogoService.Helper
{
    public class CurrencyHelper
    {
        public static Dictionary<string, string> GetCurrencies()
        {
            Dictionary<string, string> newDic = new Dictionary<string, string>();
            string today = "http://www.tcmb.gov.tr/kurlar/today.xml";
            var xmldoc = new XmlDocument();
            try
            {
                xmldoc.Load(today);
                DateTime dd = Convert.ToDateTime(xmldoc.SelectSingleNode("//Tarih_Date").Attributes["Tarih"].Value);

                string USD = xmldoc.SelectSingleNode("Tarih_Date/Currency [@Kod='USD']/ForexSelling").InnerXml;
                newDic.Add("USD", USD);
                string EUR = xmldoc.SelectSingleNode("Tarih_Date/Currency [@Kod='EUR']/ForexSelling").InnerXml;
                newDic.Add("EUR", EUR);
                string RUB = xmldoc.SelectSingleNode("Tarih_Date/Currency [@Kod='RUB']/ForexSelling").InnerXml;
                newDic.Add("RUB", RUB);
            }
            catch (Exception e)
            {
                throw e;
            }
            return newDic;
        }
        public static string GetCurrency(Dictionary<string, string> dic, string str)
        {
            //TCMB RUR değil RUB kabul ediyor
            if (str == "RUR")
            {
                str = "RUB";
            }

            var strReturn = "";
            strReturn = dic.Where(s => s.Key == str).FirstOrDefault().Value;
            if (strReturn == "" || strReturn == null)
            { strReturn = "0.0000"; } // DEFAULTED ?? 
            return strReturn;
        }

        public static string GetSiparisUrunCurrencyType(string Currency)
        {
            string siparisUrunCurrencyType;
            switch (Currency)
            {
                case "USD": // USD
                    siparisUrunCurrencyType = "1";
                    break;

                case "EUR": // EUR
                    siparisUrunCurrencyType = "20";
                    break;

                case "TL": // TL
                    siparisUrunCurrencyType = "160";
                    break;

                case "RUB":
                case "RUR":
                    siparisUrunCurrencyType = "51";
                    break;

                case "AED":
                    siparisUrunCurrencyType = "59";
                    break;

                default:
                    siparisUrunCurrencyType = "59";
                    break;
            }

            return siparisUrunCurrencyType;
        }

    }
}
