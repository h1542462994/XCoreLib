using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XCore.Windows.UI
{
    public class UWindow : Window
    {
        static UWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UWindow), new FrameworkPropertyMetadata(typeof(UWindow)));
        }

    }
}
