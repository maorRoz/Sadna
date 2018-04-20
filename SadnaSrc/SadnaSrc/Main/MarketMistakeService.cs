using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public static class MarketMistakeService
    {
        public static bool IsSimilar(string str1, string str2)
        {
            if (str1.Length != str2.Length)
            {
                return false;
            }

            int same = 0;
            for (int i = 0; i < str1.Length; i++)
            {
                if (str1[i] != str2[i])
                {
                    same--;
                }
            }
            return same >= -2;
        }
    }

}
