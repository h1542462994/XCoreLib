using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCore
{
    public static class Extensions
    {
        public static string[] SplitAny(this string obj, params char[] separator)
        {
            List<string> result = new List<string>();
            int last = -1;
            for (int i = 0; i < obj.Length; i++)
            {
                char current = obj[i];
                if (separator.Contains(current))
                {
                    if (i - last > 1)
                    {
                        result.Add(obj.Substring(last + 1, i - last - 1));
                    }
                    last = i;
                }
            }
            result.Add(obj.Substring(last + 1));
            return result.ToArray();
        }
    }

    public static class ToolKitExtensions

    {
        public static string GetAssemblyQualifiedName(this Type type)
        {
            if (type.IsArray)
            {
                Type membertype = type.GetElementType();
                return "System.Array[" + GetAssemblyQualifiedName(membertype) + "]";
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type membertype = type.GenericTypeArguments[0];
                return "System.Collections.Generic.List`1[" + GetAssemblyQualifiedName(membertype) + "]";
            }
            else
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

        }
        public static Type GetAssemblyQualifiedType(string value)
        {
            string[] v = new string[] { "System.Collections.Generic.List`1[", "System.Array[" };
            if (value.StartsWith(v[0]))
            {
                Type type = typeof(List<>);
                return type.MakeGenericType(GetAssemblyQualifiedType(value.Substring(v[0].Length, value.Length - v[0].Length - 1)));
            }
            else if (value.StartsWith(v[1]))
            {
                Type m = GetAssemblyQualifiedType(value.Substring(v[1].Length, value.Length - v[1].Length - 1));

                return m.MakeArrayType();
            }
            else
            {
                return Type.GetType(value);
            }
        }
        public static string GetFriendString(this DateTime dateTime)
        {
            var timespan = DateTime.Now - dateTime;
            if (timespan.Ticks < 0)
            {
                return dateTime.ToLongDateString();
            }
            else if (timespan.TotalMinutes < 1)
            {
                return "刚刚";
            }
            else if (timespan.TotalMinutes < 60)
            {
                return string.Format("{0}分钟前", (int)timespan.TotalMinutes);
            }
            else if (timespan.TotalHours < 24)
            {
                return string.Format("{0}小时前", (int)timespan.TotalHours);
            }
            else if (timespan.TotalDays < 7)
            {
                return string.Format("{0}天前", (int)timespan.TotalDays);
            }
            else
            {
                return dateTime.ToShortDateString();
            }
        }

        public static string Cut(this string str, int length)
        {
            if (str.Length <= length)
            {
                return str;
            }
            else
            {
                return str.Substring(0, length) + "...";
            }
        }
    }
}
