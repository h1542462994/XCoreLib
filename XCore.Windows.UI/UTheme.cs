using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XCore.Windows.UI
{
    /// <summary>
    /// 用于拓展<see cref="UControl"/>的主题设置。
    /// </summary>
    public sealed class UTheme : INotifyPropertyChanged
    {
        /// <summary>
        /// 通知客户端属性以更改
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private ThemeType themeType;
        private Color themeColor;
        private Color accentColor;

        /// <summary>
        /// 标识亮色和暗色主题
        /// </summary>
        public ThemeType ThemeType
        {
            get => themeType; set
            {
                themeType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ThemeType)));
            }
        }
        /// <summary>
        /// 主题色
        /// </summary>
        public Color ThemeColor
        {
            get => themeColor; set
            {
                themeColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ThemeColor)));
            }
        }
        /// <summary>
        /// 强调色
        /// </summary>
        public Color AccentColor
        {
            get => accentColor; set
            {
                accentColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AccentColor)));
            }
        }
    }

    /// <summary>
    /// 主题色类型.
    /// </summary>
    public enum ThemeType
    {
        /// <summary>
        /// 亮色
        /// </summary>
        Light,
        /// <summary>
        /// 暗色
        /// </summary>
        Dark,
    }
}
