using System;
using System.Text.RegularExpressions;

namespace PerrijosGatijos.Helpers
{
	public class RegexUtilities
	{
        public static bool IsValidEmail(string email)
        {
            var expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, string.Empty).Length == 0)
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }

            else
            {
                return false;
            }
        }
    }
}

