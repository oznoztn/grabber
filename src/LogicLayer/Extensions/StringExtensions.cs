using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer.Extensions
{
    public static class StringExtensions
    {
        public static string CleanOrdinals(this string str)
        {
            StringBuilder sbuilder = new StringBuilder(str);

            while (true)
            {
                char ch = sbuilder[0];

                if (char.IsNumber(ch) || char.IsSeparator(ch) || char.IsPunctuation(ch))
                {
                    sbuilder.Remove(0, 1);
                }
                else
                {
                    break;
                }
            }
            return sbuilder.ToString();
        }

        public static string CleanQuotationMarks(this string str)
        {
            StringBuilder sbuilder = new StringBuilder(str);

            if (sbuilder[0] == '\'' && sbuilder[sbuilder.Length - 1] == '\'')
            {
                sbuilder.Remove(0, 1);
                sbuilder.Remove(sbuilder.Length - 1, 1);
            }

            if (sbuilder[0] == '"' && sbuilder[sbuilder.Length - 1] == '"')
            {
                sbuilder.Remove(0, 1);
                sbuilder.Remove(sbuilder.Length - 1, 1);
            }

            if (sbuilder[0] == '‘' && sbuilder[sbuilder.Length - 1] == '’')
            {
                sbuilder.Remove(0, 1);
                sbuilder.Remove(sbuilder.Length - 1, 1);
            }


            //while (true)
            //{
            //    char cf = sbuilder[0];

            //    if (char.IsPunctuation(cf))
            //        sbuilder.Remove(0, 1);
            //    else
            //        break;
            //}

            //while (true)
            //{
            //    char cl = sbuilder[sbuilder.Length - 1];

            //    if (char.IsPunctuation(cl))
            //        sbuilder.Remove(sbuilder.Length - 1, 1);
            //    else
            //        break;
            //}

            return sbuilder.ToString();
        }
    }
}
