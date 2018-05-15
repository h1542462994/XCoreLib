using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCore.Component
{
    public class ULoggerInfo
    {
        public ULoggerInfo()
        {
        }

        public ULoggerInfo(string info, DateTime time)
        {
            Info = info;
            Time = time;
        }

        public string Info { get; set; }
        public DateTime Time { get; set; }
    }
    public class UException:ULoggerInfo
    {
        public UException()
        {
        }
        public UException(Exception exception, string info, DateTime time,bool handled):base(info,time)
        {
            Handled = handled;
            Type = exception.GetType().ToString();
            Message = exception.Message;
            Source = exception.Source;
            StackTrace = exception.StackTrace;
            if (exception.TargetSite == null)
            {
                TargetSite = "";
            }
            else
            {
                TargetSite = exception.TargetSite.ToString();
            }
        }

        public string Type { get; set; }
        public bool Handled { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string TargetSite { get; set; }
    }
}
