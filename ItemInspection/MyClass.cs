using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ItemInspection
{
    public class MyClass
    {
        public static Decimal GetNumber(string str)
        {
            if (str.IndexOf('.') == -1)
            {
                str = str + ".0";
            }
            decimal result = 0;
            try
            {
                Regex reg = new Regex(@"-?[\d]+.?[\d]+");
                Match mm = reg.Match(str);
                MatchCollection mc = reg.Matches(str);
                foreach (Match m in mc)
                {
                    result = Decimal.Parse(m.Value.ToString());
                    System.Diagnostics.Debug.WriteLine(m.Value);
                }
            }
            catch
            {

            }
            return result;
        }
    }
}
