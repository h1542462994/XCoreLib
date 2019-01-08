using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace XCore.Windows.UI
{
    /// <summary>
    /// 为<see cref="System.Windows.Controls"/>提供拓展
    /// </summary>
    public static class ControlExtension
    {
        /// <summary>
        /// 将<see cref="Frame"/>导航到指定<see cref="Page"/>.
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="pageType"></param>
        public static void NavigateTo(this Frame frame, Type pageType)
        {
            var page = Activator.CreateInstance(pageType);

            frame.Content = page;
        }

        /// <summary>
        /// 调整Color的颜色。
        /// </summary>
        /// <param name="color"></param>
        /// <param name="offset"></param>
        public static Color TranslateColor(this Color color, double offset)
        {
            color.R += (byte)((255 - color.R) * offset);
            color.G += (byte)((255 - color.G) * offset);
            color.G += (byte)((255 - color.G) * offset);

            return color;
        }
    }
}
