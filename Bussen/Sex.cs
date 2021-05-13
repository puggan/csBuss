using AppException = System.ApplicationException;

namespace Bussen
{
    public enum Sex
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
        Other = 127,
    }

    public class SexTranslations
    {
        public static string sv(Sex sex) {
            switch (sex)
            {
                case Sex.Unknown:
                    return "Okänt";
                case Sex.Male:
                    return "Man";
                case Sex.Female:
                    return "Kvinna";
                case Sex.Other:
                    return "Annat";
            }

            throw new AppException("Unreachable state?");
        }
    }
}