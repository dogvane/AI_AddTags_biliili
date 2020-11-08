using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageAddTags
{
    /// <summary>
    /// 标签拾取界面
    /// </summary>
    public partial class TagPickup : UserControl
    {
        public TagPickup()
        {
            InitializeComponent();
        }

        private static List<string> defaultTags = null;

        public string TagNames {
            get
            {
                return panelTags.TagNames;
            }
            set
            {
                panelTags.TagNames = value;
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (defaultTags == null)
            {
                defaultTags = new List<string>();

                // 初始化结束后，加载之前默认的配置
                if (File.Exists("defaultTag.txt"))
                {
                    defaultTags.AddRange(File.ReadAllLines("defaultTag.txt"));
                }
                else
                {
                    defaultTags.AddRange(new[] {"无法识别", "JK", "双马尾"});
                }
            }

            foreach (var tag in defaultTags)
            {
                panelTags.AddTag(tag);
            }

            panelTags.OnClickTag += (tagName) => { this.OnClickTag?.Invoke(tagName); };
        }

        /// <summary>
        /// 触发点击标签事件
        /// </summary>
        public event Action<string> OnClickTag;

        /// <summary>
        /// 输入框输入，相当于新建标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNewTag_KeyUp(object sender, KeyEventArgs e)
        {
            var newTags = edtNewTag.Text;
            if (string.IsNullOrEmpty(newTags))
                return;

            if (defaultTags.IndexOf(newTags) > -1)
            {
                return;
            }

            defaultTags.Add(newTags);
            File.AppendAllLines("defaultTag.txt", defaultTags);

            this.OnClickTag?.Invoke(newTags);
        }
    }
}
