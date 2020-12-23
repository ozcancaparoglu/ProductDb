namespace ProductDb.Common.Enums
{
    public class NopLanguagesToSystem
    {
        public static int ConvertLanguages(int nopLanguageId)
        {
            switch (nopLanguageId)
            {
                case 1:
                    return 2;
                case 2:
                    return 1;
                case 3:
                    return 3;
                case 4:
                    return 4;
                case 5:
                    return 5;
                case 6:
                    return 6;
                default:
                    return -1;
            }
        }
    }
}
