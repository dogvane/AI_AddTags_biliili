using BilibiliSpider.DB;
using BilibiliSpider.Entity.Database;
using BilibiliSpider.Spider.DataProcess;
using OpenCvSharp;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Window = System.Windows.Window;

namespace ImageAddTags
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (File.Exists("defaultTag.txt"))
            {
                defaultTags.AddRange(File.ReadAllLines("defaultTag.txt"));
            }
            else
            {
                defaultTags.AddRange(new[] { "无法识别", "JK", "双马尾" });
            }
        }

        private void btnNextImage_Click(object sender, RoutedEventArgs e)
        {
            if (currentShowTag == null)
            {
                if (images.Count == 0)
                    return;

                currentIndex = 0;
            }

            if (currentIndex < images.Count)
            {
                currentShowTag = images[currentIndex++];

                var image = Cv2.ImRead(currentShowTag.GetTrueFile());

                var cc = CascadeClassifierManager.Load("haarcascade_frontalface_alt2.xml");

                // 网上有看到有人先转灰度再识别，实际效果也没好多少 var gray_img = image.CvtColor(ColorConversionCodes.RGB2GRAY);

                var ract = cc.DetectMultiScale(image);

                if (ract.Length > 0)
                {
                    foreach (var r in ract)
                    {
                        Cv2.Rectangle(image, r, Scalar.Red);
                    }
                }

                var bitmap = MatToBitmapImage(image);
                imgShow.Source = bitmap;

                panelCurrentImgTags.Children.Clear();

                if (!string.IsNullOrEmpty(currentShowTag.TagsName))
                {
                    AppendTagtoCurrentTagPanel(currentShowTag.TagsName);
                }
            }
            else
            {
                // 可能要加载更多的数据了
            }
        }

        public BitmapImage MatToBitmapImage(Mat image)
        {
            System.Drawing.Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
            using var ms = new MemoryStream();

            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            BitmapImage result = new BitmapImage();
            result.BeginInit();
            result.CacheOption = BitmapCacheOption.OnLoad;
            result.StreamSource = ms;
            result.EndInit();

            return result;
        }

        private ImageTag currentShowTag = null;

        private List<ImageTag> images = new List<ImageTag>();
        private int currentIndex = -1;

        private List<string> defaultTags = new List<string>();

        private void Window_Loaded(object sender, EventArgs e)
        {
            // 窗口初始化结束
            using var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili);
            var list = db.Select<ImageTag>().Take(100);

            foreach (var item in list)
            {
                var trueFile = item.GetTrueFile();

                if (File.Exists(trueFile))
                {
                    images.Add(item);
                }
            }

            LoadDefaultTagsToShow();

            btnNextImage_Click(null, null);
        }

        private void LoadDefaultTagsToShow()
        {
            paneldefaultTags.Children.Clear();

            foreach (var tagName in defaultTags)
            {
                AppendTagtoNewTagPanel(tagName);
            }
        }

        private void AppendTagtoNewTagPanel(string tagName)
        {
            // BorderBrush="#FFF32222" BorderThickness="1,1,1,1" Margin="5,0,5,0"

            var lab = new Label()
            {
                Content = tagName,
                BorderBrush = Brushes.Red,
                BorderThickness = new Thickness(1, 1, 1, 1),
                Margin = new Thickness(5, 2, 5, 2),
            };

            lab.MouseUp += OnLableNewTagMouseUp;
            lab.MouseDoubleClick += OnLableDelTags;

            paneldefaultTags.Children.Add(lab);
        }

        private void AppendTagtoCurrentTagPanel(string tagName)
        {
            if (tagName.IndexOf(',') > -1)
            {
                foreach (var t in tagName.Split(','))
                {
                    AppendTagtoCurrentTagPanel(t);
                }
            }
            else
            {
                var lab = new Label()
                {
                    Content = tagName,
                    BorderBrush = Brushes.Red,
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    Margin = new Thickness(5, 2, 5, 2),
                };

                lab.MouseUp += OnCurrentRmoveTag;

                panelCurrentImgTags.Children.Add(lab);
            }
        }

        private void OnCurrentRmoveTag(object sender, MouseButtonEventArgs e)
        {
            var clickLabel = e.Source as Label;

            if (clickLabel != null)
            {
                var removeTagName = clickLabel.Content.ToString();

                var exits = currentShowTag.TagsName.Split(',').ToList();
                exits.Remove(removeTagName);
                currentShowTag.TagsName = ToCSV(exits.ToArray());

                panelCurrentImgTags.Children.Remove(clickLabel);
            }
        }

        private void OnLableDelTags(object sender, MouseButtonEventArgs e)
        {
            // 删除一个标签
            var clickLabel = e.Source as Label;

            if (clickLabel != null)
            {
                var tagName = clickLabel.Content.ToString();
                defaultTags.Remove(tagName);
                File.WriteAllLines("defaultTag.txt", defaultTags);
                paneldefaultTags.Children.Remove(clickLabel);
            }
        }

        private void OnLableNewTagMouseUp(object sender, MouseButtonEventArgs e)
        {
            var clickLabel = e.Source as Label;

            if (clickLabel != null)
            {
                var newTagName = clickLabel.Content.ToString();

                // 将当前标签，加入图片的tag里
                if (currentShowTag != null)
                {
                    var currentTags = currentShowTag.TagsName?.Split(',');

                    if (currentTags == null || currentTags.Length == 0)
                    {
                        currentShowTag.TagsName = newTagName;
                        AppendTagtoCurrentTagPanel(newTagName);
                    }
                    else
                    {
                        if (!currentTags.Contains(newTagName))
                        {
                            currentShowTag.TagsName += "," + currentShowTag;
                            AppendTagtoCurrentTagPanel(newTagName);
                        }
                    }
                }
            }
        }

        private string ToCSV(string[] arr)
        {
            if (arr == null || arr.Length == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (var a in arr)
            {
                sb.Append(a).Append(',');
            }

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        private void txtNewTag_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // 如果是按回车，说明是确认输入一个新的tag
                var newTag = txtNewTag.Text?.Trim();

                if (!string.IsNullOrEmpty(newTag))
                {
                    var current = images[currentIndex];
                    var tags = current.TagsName?.Split(',');

                    if (tags != null && tags.Contains(newTag))
                    {
                        return;
                    }

                    if (tags == null || tags.Length == 0)
                    {
                        current.TagsName = newTag;
                    }
                    else
                    {
                        current.TagsName += "," + newTag;
                    }

                    txtNewTag.Text = string.Empty;

                    if (!defaultTags.Contains(newTag))
                    {
                        defaultTags.Add(newTag);
                        File.WriteAllLines("defaultTag.txt", defaultTags);
                        AppendTagtoNewTagPanel(newTag);
                    }
                }
            }
        }

        /// <summary>
        /// 保存当前的状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (currentShowTag != null)
            {
                using var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili);
                db.Update(currentShowTag);
            }
        }
    }
}