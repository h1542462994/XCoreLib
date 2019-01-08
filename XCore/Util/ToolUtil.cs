using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XCore.Util
{
    public static class ToolUtil
    {
        public static string GetNewToken()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
        public static string MD5Encrypt32(string password)
        {
            string cl = password;
            //string pwd = "";
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in s)
            {
                stringBuilder.AppendFormat("{0:X2}", item);
            }


            return stringBuilder.ToString();
        }

        public static string JoinString(string seperator, IEnumerable<string> obj)
        {
            if (obj == null || obj.Count() == 0)
            {
                return "";
            }
            else
            {
                return string.Join(seperator, obj);
            }
        }
        public static List<string> SplitString(char seperator, string obj)
        {
            if (obj == null)
            {
                return new List<string>();
            }
            else
            {
                return obj.Split(seperator).ToList();
            }
        }
    }
}
