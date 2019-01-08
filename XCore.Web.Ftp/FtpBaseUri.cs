using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCore.Web.Ftp
{
    /// <summary>
    /// 表示Ftp依赖的Credentials及ServerIP.
    /// </summary>
    public class FtpBaseUri:IEquatable<FtpBaseUri>
    {
        public FtpBaseUri(string serverIP, string userID="", string password="")
        {
            ServerIP = serverIP;
            UserID = userID;
            Password = password;
        }

        public string ServerIP { get; private set; }
        public string UserID { get; private set; }
        public string Password { get; private set; }

        public bool Equals(FtpBaseUri other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                return ServerIP == other.ServerIP && UserID == other.UserID && Password == other.Password;
            }
        }
    }
}
