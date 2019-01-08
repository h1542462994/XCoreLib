using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XCore.Web.Ftp
{
    public class FtpDownloadFileTask : AsyncTask<FtpFileInfo, ChannelProcessInfo>
    {
        public FtpDownloadFileTask() { }
        public FtpDownloadFileTask(FtpFileInfo key, string uri, bool isFullName = false) : this( key, new ChannelProcessInfo(), uri, isFullName)
        {

        }
        public FtpDownloadFileTask(FtpFileInfo key, ChannelProcessInfo info, string uri, bool isFullName = false) : base( key)
        {
            Uri = uri;
            IsFullName = isFullName;
            Info = info;
        }
        public override void Doing()
        {
            string fileName;
            if (!IsFullName)
            {
                fileName = Uri + Key. ParentUri + Key. DisplayName;
            }
            else
            {
                fileName = Uri;
            }
            FtpFileSystemInfo.SplitUri(fileName, out string d, out string dn);
            if (!Directory.Exists(d))
            {
                Directory.CreateDirectory(d);
            }
            using (FileStream stream = new FileStream(fileName, FileMode.Append))
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(Key.Uri));
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.UseBinary = true;
                request.UsePassive = false;
                request.ContentOffset = stream.Length;
                Info.Current = stream.Length;
                request.Credentials = new NetworkCredential(Key. BaseUri.UserID, Key. BaseUri.Password);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    using (Stream ftpstream = response.GetResponseStream())
                    {
                        int bufferSize = 2048;
                        int readCount;
                        byte[] buffer = new byte[bufferSize];
                        stream.Position = stream.Length;
                        Info.Size = Key.Size;
                        //指定内部取消.
                        while (true)
                        {
                            if (isCanceling)
                            {
                                ProcessState = ProcessState.Waiting;
                                isCanceling = false;
                                break;
                            }
                            else
                            {
                                readCount = ftpstream.Read(buffer, 0, bufferSize);
                                if (readCount > 0)
                                {
                                    stream.Write(buffer, 0, readCount);
                                    Info.Current += readCount;
                                    ProcessState = ProcessState.Doing;
                                }
                                else
                                {
                                    ProcessState = ProcessState.Completed;
                                    break;
                                }
                            }
        
                        }
                    }
                }
            }

        }
        public string Uri { get; set; }
        public bool IsFullName { get; set; }
    }
    public class ChannelProcessInfo
    {
        public long Current { get; set; }
        public long Size { get; set; }
    }
}
