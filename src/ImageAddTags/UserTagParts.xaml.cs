using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BilibiliSpider.Entity.Database;

namespace ImageAddTags
{
    //public class PopupEx : Popup
    //{
    //    /// <summary>  
    //    /// 是否窗口随动，默认为随动（true）  
    //    /// </summary>  
    //    public bool IsPositionUpdate
    //    {
    //        get { return (bool)GetValue(IsPositionUpdateProperty); }
    //        set { SetValue(IsPositionUpdateProperty, value); }
    //    }

    //    public static readonly DependencyProperty IsPositionUpdateProperty =
    //        DependencyProperty.Register("IsPositionUpdate", typeof(bool), typeof(PopupEx), new PropertyMetadata(true, new PropertyChangedCallback(IsPositionUpdateChanged)));

    //    private static void IsPositionUpdateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        (d as PopupEx).pup_Loaded(d as PopupEx, null);
    //    }

    //    /// <summary>  
    //    /// 加载窗口随动事件  
    //    /// </summary>  
    //    public PopupEx()
    //    {
    //        this.Loaded += pup_Loaded;
    //    }

    //    /// <summary>  
    //    /// 加载窗口随动事件  
    //    /// </summary>  
    //    private void pup_Loaded(object sender, RoutedEventArgs e)
    //    {
    //        Popup pup = sender as Popup;
    //        var win = VisualTreeHelper.GetParent(pup);
    //        while (win != null && (win as Window) == null)
    //        {
    //            win = VisualTreeHelper.GetParent(win);
    //        }
    //        if ((win as Window) != null)
    //        {
    //            (win as Window).LocationChanged -= PositionChanged;
    //            (win as Window).SizeChanged -= PositionChanged;
    //            if (IsPositionUpdate)
    //            {
    //                (win as Window).LocationChanged += PositionChanged;
    //                (win as Window).SizeChanged += PositionChanged;
    //            }
    //        }
    //    }

    //    /// <summary>  
    //    /// 刷新位置  
    //    /// </summary>  
    //    private void PositionChanged(object sender, EventArgs e)
    //    {
    //        try
    //        {
    //            var method = typeof(Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    //            if (this.IsOpen)
    //            {
    //                method.Invoke(this, null);
    //            }
    //        }
    //        catch
    //        {
    //            return;
    //        }
    //    }

    //    //是否最前默认为非最前（false）  
    //    public static DependencyProperty TopmostProperty = Window.TopmostProperty.AddOwner(typeof(Popup), new FrameworkPropertyMetadata(false, OnTopmostChanged));
    //    public bool Topmost
    //    {
    //        get { return (bool)GetValue(TopmostProperty); }
    //        set { SetValue(TopmostProperty, value); }
    //    }
    //    private static void OnTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    //    {
    //        (obj as PopupEx).UpdateWindow();
    //    }

    //    /// <summary>  
    //    /// 重写拉开方法，置于非最前  
    //    /// </summary>  
    //    /// <param name="e"></param>  
    //    protected override void OnOpened(EventArgs e)
    //    {
    //        UpdateWindow();
    //    }

    //    /// <summary>  
    //    /// 刷新Popup层级  
    //    /// </summary>  
    //    private void UpdateWindow()
    //    {
    //        var hwnd = ((HwndSource)PresentationSource.FromVisual(this.Child)).Handle;
    //        RECT rect;
    //        if (NativeMethods.GetWindowRect(hwnd, out rect))
    //        {
    //            NativeMethods.SetWindowPos(hwnd, Topmost ? -1 : -2, rect.Left, rect.Top, (int)this.Width, (int)this.Height, 0);
    //        }
    //    }

    //    [StructLayout(LayoutKind.Sequential)]
    //    public struct RECT
    //    {
    //        public int Left;
    //        public int Top;
    //        public int Right;
    //        public int Bottom;
    //    }
    //    #region P/Invoke imports & definitions  
    //    public static class NativeMethods
    //    {


    //        [DllImport("user32.dll")]
    //        [return: MarshalAs(UnmanagedType.Bool)]
    //        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    //        [DllImport("user32", EntryPoint = "SetWindowPos")]
    //        internal static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);
    //    }
    //    #endregion
    //}

    /// <summary>
    /// UserTagParts.xaml 的交互逻辑
    /// </summary>
    public partial class UserTagParts : UserControl
    {
        public UserTagParts()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化控件要显示的内容
        /// </summary>
        /// <param name="item"></param>
        public void InitData(TagPart item)
        {
            m_item = item;
            tagPickup.OnClickTag += TagPickup_OnClickTag;

            if (m_item.State == 2)
            {
                btnAddTag.Visibility = Visibility.Collapsed;
            }

            panelTags.TagNames = item.TagNames;
        }

        private void TagPickup_OnClickTag(string tagName)
        {
            Console.WriteLine(tagName);
            // 这里是操作增加的
            panelTags.AddTag(tagName);
            menuPop1.IsOpen = false;
            menuPop1.IsOpen = true;

            m_item.State = 1;
            m_item.TagNames = panelTags.TagNames;
        }

        private TagPart m_item;

        private void labAddTag_Click(object sender, RoutedEventArgs e)
        {
            // 如果点击添加，则显示 PickUp 的面板
            // this.btnAddTag.
            menuPop1.IsOpen = true;
        }

        private void labAddTag_MouseEnter(object sender, MouseEventArgs e)
        {
            // 鼠标移入，显示tab面板
            menuPop1.IsOpen = true;
            // 
        }

        /// <summary>
        /// 在tag上点击事件
        /// </summary>
        /// <param name="tagName"></param>
        private void panelTags_OnClickTag(string tagName)
        {
            // 这里的点击，就意味着需要删除该tag
            panelTags.RemoveTag(tagName);
            m_item.TagNames = panelTags.TagNames;
        }

        /// <summary>
        /// 将当前的图片素材设置为无效素材
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNotUse_Click(object sender, RoutedEventArgs e)
        {
            if(btnAddTag.Visibility == Visibility.Visible)
                btnAddTag.Visibility = Visibility.Collapsed;

            m_item.State = 2;
        }
    }
}
