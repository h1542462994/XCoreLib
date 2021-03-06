﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XCore.Web.Ftp;
using System.Xml.Linq;

namespace XCoreLibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                string t = Console.ReadLine();

                switch (t)
                {
                    case "list":
                        FtpFolderInfo a = new FtpFolderInfo(new FtpBaseUri("192.168.1.1"), "");
                        foreach (var item in a.GetDetails())
                        {
                            Console.WriteLine(item);
                        }
                        a.GoToUri("教学处/");
                        foreach (var item in a.GetDetails())
                        {
                            Console.WriteLine(item);
                        }
                        break;
                    case "create":
                        FtpFolderInfo folderInfo = new FtpFolderInfo(new FtpBaseUri("192.168.8.215"), "教学处/2018届/");
                        foreach (var item in folderInfo.GetFtpFileSystemInfos())
                        {
                            Console.WriteLine(item.DisplayName);
                            if (item is FtpFileInfo info)
                            {
                                Console.WriteLine(info.Extension);
                            }
                        }
                        break;
                    case "test":
                        FtpFolderInfo f = new FtpFolderInfo(new FtpBaseUri("192.168.8.215"), "");
                        f.GetFtpFileSystemInfos();
                        break;
                }
            }

        }

        static void Foreach(FtpFolderInfo info)
        {
            Console.WriteLine(">>>>>>>>" + info.Uri);
            foreach (var item in info.GetFtpFiles())
            {
                Console.WriteLine(item.Uri);
                if (FileExtensions.Pictures.Contains(item.Extension.ToLower()))
                {
                    item.Download(AppDomain.CurrentDomain.BaseDirectory + "Pic/");
                    Console.WriteLine("download");

                }
            }
            foreach (var item in info.GetFtpFolders())
            {
                Foreach(item);
            }
        }
    }



}
