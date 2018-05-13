using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCore.UI
{
    public class MessageBoxButtonMetadata
    {
        public MessageBoxButtonMetadata(string displayText, Action callBack)
        {
            DisplayText = displayText;
            CallBack = callBack;
        }   
        public string DisplayText { get; set; }
        public Action CallBack { get; set; }
    }
}
