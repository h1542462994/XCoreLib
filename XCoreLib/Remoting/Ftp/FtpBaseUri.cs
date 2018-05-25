using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCore.Remoting.Ftp
{
    public class FtpBaseUri
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
    }
}
