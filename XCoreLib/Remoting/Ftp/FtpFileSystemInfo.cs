using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace XCore.Remoting.Ftp
{
    /// <summary>
    /// 表示一个ftp对象.
    /// </summary>
    public abstract class FtpFileSystemInfo : IEquatable<FtpFileSystemInfo>
    {
        protected FtpFileSystemInfo()
        {
        }
        [XmlIgnore]
        public FtpBaseUri BaseUri { get; set; }
        public string ParentUri { get; set; }
        public string DisplayName { get; set; }
        public DateTime LastAccessTime { get; set; }
        public long Size { get; set; }
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
        public void GoToUri(string relativeUri)
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
        public static void SplitUri(string relativeUri, out string parentUri, out string displayName)
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
                    parentUri = p.Substring(0, index + 1);
                    displayName = p.Substring(index + 1);
                }
            }

        }
        protected FtpFileSystemInfo TranslateTo(string info)
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

        public bool Equals(FtpFileSystemInfo other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                return BaseUri.Equals(other) && Uri.Equals(other.Uri);
            }
        }
    }
    /// <summary>
    /// 表示一个ftp文件夹.
    /// </summary>
    public class FtpFolderInfo : FtpFileSystemInfo
    {
        public FtpFolderInfo()
        {
        }
        public FtpFolderInfo(FtpBaseUri baseUri, string relativeUri = "")
        {
            BaseUri = baseUri;
            GoToUri(relativeUri);
        }
        /// <summary>
        /// 获得当前路径下ftp对象的数组.
        /// </summary>
        /// <returns></returns>
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

        public FtpFileInfo GetFtpFile(string displayName)
        {
            var x = from item in GetFtpFiles() where item.DisplayName == displayName select item;
            if (x.Count() == 0)
            {
                return null;
            }
            else
            {
                return x.First();
            }
        }
        /// <summary>
        /// 根据LIST协议获取具体信息.
        /// </summary>
        /// <returns></returns>
        public string[] GetDetails()
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                if (ex.Message == "无法连接到远程服务器")
                {
                    throw ex;
                }
                return null;
            }
        }
    }
    /// <summary>
    /// 表示一个ftp文件.
    /// </summary>
    public class FtpFileInfo : FtpFileSystemInfo
    {
        public FtpFileInfo()
        {
        }
        public FtpFileInfo(FtpBaseUri baseUri, string relativeUri = "")
        {
            BaseUri = baseUri;
            GoToUri(relativeUri);
        }
        public string Extension => GetExtension(DisplayName);
        public void Download(string uri, bool isFullName = false)
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
            SplitUri(fileName, out string d, out string dn);
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
        /// <summary>
        /// 创建一个可断点续传的文件下载任务.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="isFullName"></param>
        /// <returns></returns>
        public FtpDownloadFileTask CreateDownloadTask(string uri, bool isFullName = false)
        {
            return new FtpDownloadFileTask(this, uri, isFullName);
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
