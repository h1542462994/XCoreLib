using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XCore.UI
{
    public static class MessageBox
    {
        public static void Show(Window window, string message)
        {
            Current = GetGrid();
            if (window.Content is Grid grid)
            {
                grid.Children.Add(Current);
            }
        }
        static Grid Current { get; set; }
        public static Grid GetGrid()
        {
            Grid grid = new Grid()
            {
                Background = new SolidColorBrush(Color.FromArgb(153, 0, 0, 0)),
            };
            MsgBox msg = new MsgBox()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            grid.Children.Add(msg);


            return grid;
        }
    }
}
