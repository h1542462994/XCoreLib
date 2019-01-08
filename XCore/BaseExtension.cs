using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCore
{
    public static class BaseExtension
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
