using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BilibiliSpider.Common;
using BilibiliSpider.DB;
using BilibiliSpider.Entity.Database;
using ImageAddTags.DataSet;
using ServiceStack.OrmLite;
using Path = System.IO.Path;

namespace ImageAddTags
{
    /// <summary>
    /// Output.xaml 的交互逻辑
    /// </summary>
    public partial class Output : Window
    {
        public Output()
        {
            InitializeComponent();
        }



        private void btnOutput_Click(object sender, RoutedEventArgs e)
        {
            TagsDataSet.WriteSourceData(tags, Path.Combine(Utils.DefaultDataFolder, "TrainData/bilibili.tags"));
        }

        private List<ImageTag> tags = new List<ImageTag>();


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 加载数据
            using var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili);
            tags = db.Select<ImageTag>(o => o.Status == "completed");
            labOutput.Content = $"当前有效tag数量：{tags.Count}";
        }


    }
}
