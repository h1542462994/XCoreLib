using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XCore.Remoting.Ftp;

namespace FtpWpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public FtpDownloadFileTask downloadFileTask;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private void DownloadFileTask_ProcessChanged(object sender, ProcessChangedEventArgs<ChannelProcessInfo> e)
        {
            Dispatcher.Invoke(() =>
            {
                Tbx1.Text = string.Format("{0} {1}", e.Info.Current, e.Info.Size);
            });

        }

        private void FtpDownload_Click(object sender, RoutedEventArgs e)
        {

            downloadFileTask.StartAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            downloadFileTask.Stop();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FtpFolderInfo ftpFolder = new FtpFolderInfo(new FtpBaseUri("192.168.8.215"), "soft/");

            FtpFileInfo ftpFile = ftpFolder.GetFtpFile("绘声绘影.rar");
            Console.WriteLine("1");
            downloadFileTask = new FtpDownloadFileTask( ftpFile, AppDomain.CurrentDomain.BaseDirectory + "File/");
            downloadFileTask.ProcessChanged += DownloadFileTask_ProcessChanged;
        }
    }
}
