using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCore
{
    public static class Extension
    {
        public static string GetAssemblyQualifiedName(this Type type)
        {
            if (type.Assembly == System.Reflection.Assembly.GetAssembly(typeof(int)))
            {
                return type.ToString();
            }
            else
            {
                return type.AssemblyQualifiedName;
            }
        }
        public static string[] SplitAny(this string obj,params char[] separator)
        {
            List<string> result = new List<string>();
            int last = -1;
            for (int i = 0; i < obj.Length; i++)
            {
                char current = obj[i];
                if (separator.Contains(current))
                {
                    if (i-last>1)
                    {
                        result.Add(obj.Substring(last+1, i - last - 1));
                    }
                    last = i;
                }
            }
            result.Add( obj.Substring(last + 1));
            return result.ToArray();
        }
    }
}
