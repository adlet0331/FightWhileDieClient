using System;
using System.Globalization;

namespace Utils
{
    public class IntToUnitString
    {
        public static string ToString(int val)
        {
            if (val >= 1000000000)
            {
                return Math.Round((val / 1000000000.0f), 1).ToString(CultureInfo.InvariantCulture) + "B";
            }
            else if (val >= 1000000)
            {
                return Math.Round((val / 1000000.0f), 1).ToString(CultureInfo.InvariantCulture) + "M";
            }
            else if (val >= 1000)
            {
                return Math.Round((val / 1000.0f), 1).ToString(CultureInfo.InvariantCulture) + "K";
            }
            else if (val >= 0)
            {
                return val.ToString();
            }
            else
            {
                return "Error";
            }
        }
    }
}