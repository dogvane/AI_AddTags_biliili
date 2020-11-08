using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageAddTags
{
    /// <summary>
    /// TagPanelShow.xaml 的交互逻辑
    /// </summary>
    public partial class TagPanelShow : UserControl
    {
        public TagPanelShow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始设置标签项目，标签用逗号分隔
        /// </summary>
        /// <param name="tags"></param>
        public void SetTagsName(string tags)
        {
            if (string.IsNullOrEmpty(tags))
                return;

            panelTags.Children.Clear();

            foreach (var item in tags.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                AppendNewTag(item);
            }
        }

        private void AppendNewTag(string item)
        {
            var lab = new Label()
            {
                Content = item,
                BorderBrush = Brushes.Red,
                BorderThickness = new Thickness(1, 1, 1, 1),
                Margin = new Thickness(5, 2, 5, 2),
            };

            lab.MouseUp += Lab_MouseUp;

            panelTags.Children.Add(lab);
        }

        public void AddTag(string tagName)
        {
            foreach (Label label in panelTags.Children)
            {
                // 确认有没有重复的
                if (label.Content.ToString() == tagName)
                    return;
            }

            AppendNewTag(tagName);
        }

        public void RemoveTag(string tagName)
        {
            Label remove = null;

            foreach (Label label in panelTags.Children)
            {
                // 确认有没有重复的
                var ctag = label.Content.ToString();
                if (ctag == tagName)
                {
                    remove = label;
                    break;
                }
            }

            if (remove != null)
            {
                panelTags.Children.Remove(remove);
            }
        }

        /// <summary>
        /// 获得当前在界面里展示的所有标签项
        /// 逗号分隔
        /// </summary>
        public string TagNames
        {
            get
            {
                var ret = "";

                foreach (Label label in panelTags.Children)
                {
                    ret += label.Content + ",";
                }

                if (ret.Length > 0)
                {
                    ret = ret.TrimEnd(',');
                }

                return ret;
            }
            set => SetTagsName(value);
        }

        private void Lab_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // 点击标签后的操作
            var lab = sender as Label;
            if (lab == null)
                return;

            var tagName = lab.Content.ToString();
            OnClickTag?.Invoke(tagName);
        }

        
        /// <summary>
        /// 触发点击标签事件
        /// </summary>
        public event Action<string>  OnClickTag ;
    }
}
