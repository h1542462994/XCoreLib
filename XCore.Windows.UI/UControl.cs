using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace XCore.Windows.UI
{
    /// <summary>
    /// 继承于<see cref="ContentControl"/>兼容主题颜色设置和点击事件的控件.
    /// </summary>
    public class UControl:ContentControl
    {
        private bool _isTapped = false;
        /// <summary>
        /// 主题
        /// </summary>
        public UTheme Theme
        {
            get { return (UTheme)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }
        /// <summary>
        /// 表示左键点击事件
        /// </summary>
        public event RoutedEventHandler Tapped
        {
            add
            {
                AddHandler(TappedEvent,value);
            }
            remove
            {
                RemoveHandler(TappedEvent, value);
            }
        }
        /// <summary>
        /// 表示鼠标按下
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            _isTapped = true;
        }
        /// <summary>
        /// 表示鼠标离开控件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            _isTapped = false;
        }
        /// <summary>
        /// 表示鼠标弹起
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isTapped)
            {
                RaiseEvent(new RoutedEventArgs(TappedEvent, this));
            }
        }

        #region static
        /// <summary>
        /// 主题依赖属性
        /// </summary>
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(UTheme), typeof(UControl), new PropertyMetadata(null));
        /// <summary>
        /// 定义鼠标点击事件
        /// </summary>
        public static RoutedEvent TappedEvent = EventManager.RegisterRoutedEvent("Tapped",      RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UControl));
        #endregion
    }
}
