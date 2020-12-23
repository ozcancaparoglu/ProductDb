using System.Globalization;

namespace ProductDb.Common.Helpers
{
    public class Convertions
    {
        public static decimal PriceConvert(string priceStr)
        {
            if (decimal.TryParse(priceStr, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out decimal res))
                return res;
            return 0;
        }

        public static string SubStringWithMaxCharNumber(string str, int lenght)
        {
            if (str.Length > lenght)
            {
                str = str.Substring(0, lenght);
                return str.Substring(0, str.LastIndexOf(" "));
            }

            return str;
        }

        public static string ReplaceTurkishCharacters(string incomingText)
        {
            incomingText = incomingText.Replace("Ş", "S");
            incomingText = incomingText.Replace("İ", "I");
            incomingText = incomingText.Replace("Ö", "O");
            incomingText = incomingText.Replace("Ç", "C");
            incomingText = incomingText.Replace("Ğ", "G");
            incomingText = incomingText.Replace("Ü", "U");
            incomingText = incomingText.Replace("ü", "u");
            incomingText = incomingText.Replace("ş", "s");
            incomingText = incomingText.Replace("ç", "c");
            incomingText = incomingText.Replace("ö", "o");
            incomingText = incomingText.Replace("ı", "i");
            incomingText = incomingText.Replace("ğ", "g");

            return incomingText;
        }

    }
}
