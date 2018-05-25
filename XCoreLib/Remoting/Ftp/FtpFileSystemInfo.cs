using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace XCore.Remoting.Ftp
{
    /// <summary>
    /// 表示一个ftp对象.
    /// </summary>
    public abstract class FtpFileSystemInfo
    {
        protected FtpFileSystemInfo()
        {
        }
        public FtpBaseUri BaseUri { get; set; }
        public string ParentUri { get; set; }
        public string DisplayName { get; set; }
        public DateTime LastAccessTime { get; set; }
        public long Size { get; set; }
        public object Tag { get; set; }
        public string Uri
        {
            get
            {
                if (this is FtpFileInfo)
                {
                    return "ftp://" + BaseUri.ServerIP + "/" + ParentUri + DisplayName;
                }
                else
                {
                    if (DisplayName == "")
                    {
                        return "ftp://" + BaseUri.ServerIP + "/";
                    }
                    else
                    {
                        return "ftp://" + BaseUri.ServerIP + "/" + ParentUri + DisplayName + "/";
                    }
                }
            }
        }
    }
    public class FtpFolderInfo : FtpFileSystemInfo
    {
        public FtpFolderInfo()
        {
        }
        public FtpFolderInfo(FtpBaseUri baseUri, string relativeUri = "")
        {
            BaseUri = baseUri;
            GoToDirectory(relativeUri);
        }
        public FtpFileSystemInfo[] GetFtpFileSystemInfos()
        {
            try
            {
                List<FtpFileSystemInfo> infos = new List<FtpFileSystemInfo>();
                FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create(new Uri(Uri));
                ftp.Credentials = new NetworkCredential(BaseUri.UserID, BaseUri.Password);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftp.UsePassive = false;
                using (WebResponse response = ftp.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default))
                    {
                        List<string> result = new List<string>();
                        while (!reader.EndOfStream)
                        {
                            result.Add(reader.ReadLine());
                        }

                        foreach (var item in result)
                        {
                            FtpFileSystemInfo ftpFileSystemInfo = TranslateTo(item);
                            if (ftpFileSystemInfo != null)
                            {
                                infos.Add(ftpFileSystemInfo);
                            }
                        }
                    }
                }
                return infos.ToArray();
            }
            catch (Exception)
            {
                return null;
            }

        }
        public FtpFolderInfo[] GetFtpFolders() => GetFtpFileSystemInfos().OfType<FtpFolderInfo>().ToArray();
        public FtpFileInfo[] GetFtpFiles() => GetFtpFileSystemInfos().OfType<FtpFileInfo>().ToArray();
        public void GoToDirectory(string relativeUri)
        {
            if (relativeUri != "")
            {
                if (!(relativeUri.Last() == '\\' || relativeUri.Last() == '/'))
                {
                    relativeUri = relativeUri + "/";
                }
                relativeUri = relativeUri.Replace('\\', '/');
            }
            SplitUri(relativeUri, out string a, out string b);
            ParentUri = a; DisplayName = b;
        }

        /// <summary>
        /// 根据LIST协议获取具体信息.
        /// </summary>
        /// <returns></returns>
        public string[] GetDetails()
        {
            //try
            //{
                FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create(new Uri(Uri));
                ftp.Credentials = new NetworkCredential(BaseUri.UserID, BaseUri.Password);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftp.UsePassive = false;
                Console.WriteLine(Uri);
                using (WebResponse response = ftp.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default))
                    {
                        List<string> result = new List<string>();
                        while (!reader.EndOfStream)
                        {
                            result.Add(reader.ReadLine());

                        }
                        return result.ToArray();
                    }
                }
            //}
            //catch (Exception)
            //{
            //    return null;
            //}
        }
        void SplitUri(string relativeUri, out string parentUri, out string displayName)
        {
            if (relativeUri == "")
            {
                parentUri = "";
                displayName = "";
            }
            else
            {
                string p = relativeUri.Remove(relativeUri.Length - 1);
                if (!p.Contains('/'))
                {
                    parentUri = "";
                    displayName = p;
                }
                else
                {
                    int index = p.LastIndexOf('/');
                    parentUri = p.Substring(0, index +1);
                    displayName = p.Substring(index + 1);
                }
            }

        }
        FtpFileSystemInfo TranslateTo(string info)
        {
            string[] infos = info.SplitAny(' ');
            if (infos[0] == "drw-rw-rw-")
            {
                if (infos[8] == "." || infos[8] == "..")
                {
                    return null;
                }
                else
                {
                    return new FtpFolderInfo()
                    {
                        BaseUri = this.BaseUri,
                        ParentUri = this.ParentUri + DisplayName + '/',
                        DisplayName = infos[8],
                        Size = 0
                    };
                }
            }
            else
            {
                return new FtpFileInfo()
                {
                    BaseUri = this.BaseUri,
                    ParentUri = this.ParentUri + DisplayName + '/',
                    DisplayName = infos[8],
                    Size = long.Parse(infos[4])
                };
            }
        }
    }
    public class FtpFileInfo : FtpFileSystemInfo
    {
        public FtpFileInfo()
        {
        }
        public string Extension => GetExtension(DisplayName);
        public void Download(string uri, bool isFullName = false)
        {
            try
            {
                string fileName;
                if (!isFullName)
                {
                    fileName = uri + ParentUri + DisplayName;
                }
                else
                {
                    fileName = uri;
                }
                string d = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(d))
                {
                    Directory.CreateDirectory(d);
                }
                using (FileStream stream = new FileStream(fileName, FileMode.Create))
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(Uri));
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UseBinary = true;
                    request.UsePassive = false;
                    request.Credentials = new NetworkCredential(BaseUri.UserID, BaseUri.Password);
                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        using (Stream ftpstream = response.GetResponseStream())
                        {
                            long cl = response.ContentLength;
                            int bufferSize = 2048;
                            int readCount;
                            byte[] buffer = new byte[bufferSize];
                            readCount = ftpstream.Read(buffer, 0, bufferSize);
                            while (readCount > 0)
                            {
                                stream.Write(buffer, 0, readCount);
                                readCount = ftpstream.Read(buffer, 0, bufferSize);
                            }

                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private string GetExtension(string displayName)
        {
            if (displayName.Contains('.'))
            {
                string[] t = displayName.Split('.');
                return "." + t[t.Length - 1];
            }
            else
            {
                return "";
            }
        }
    }
}
