using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HouseDataCleaning
{
    public static class ParsingExtension
    {
        public static int? TryParseInt(this string value)
        {
            if (value == null) return null;
            int result = 0;
            if (int.TryParse(value, out result))
            {
                return result;
            }
            return null;
        }

        static Regex _k = new Regex(@" *k$", RegexOptions.IgnoreCase);
        static Regex _m = new Regex(@" *m$", RegexOptions.IgnoreCase);
        public static double? TryParsePrice(this string value)
        {
            if (value == null) return null;

            value = _k.Replace(value, "000");
            value = _m.Replace(value, "000000");

            double result = 0;
            if (double.TryParse(value, out result))
            {
                return result;
            }
            return null;

        }
    }
}
