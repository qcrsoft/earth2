using System;
using System.Collections.Generic;
using System.Text;

namespace Earth.Library
{
    public static class Extention
    {
        public static int IndexOf(this StringBuilder sb, string value)
        {
            int ret = -1;
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == value[0])
                {
                    int k = 1;
                    for (int j = 1; j < value.Length; j++)
                    {
                        if (sb[i + j] == value[j])
                            k++;
                        else
                            break;
                    }
                    if (value.Length == k)
                    {
                        ret = i;
                        break;
                    }
                    else
                        ret = -1;
                }
            }
            return ret;
        }

        public static string Substring(this StringBuilder sb, int startIndex, int length)
        {
            char[] cs = new char[length];
            sb.CopyTo(startIndex, cs, 0, length);
            return new string(cs);
        }
    }
}
