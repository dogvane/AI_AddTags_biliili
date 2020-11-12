using BilibiliSpider.DB;
using BilibiliSpider.Entity.Database;
using BilibiliSpider.Spider.DataProcess;
using OpenCvSharp;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BilibiliSpider.Common;
using ImageAddTags.Common;
using ImageAddTags.DataSet;
using OpenCvSharp.Extensions;
using ServiceStack;
using Point = OpenCvSharp.Point;
using Window = System.Windows.Window;
using Rect = OpenCvSharp.Rect;

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

        private void btnUpImage_Click(object sender, RoutedEventArgs e)
        {
            if (currentShowTag == null)
            {
                if (images.Count == 0)
                    return;

                currentIndex = 0;
            }

            if (currentIndex <= 0)
            {
                return;
            }
            currentIndex--;

            LoadPageShow();
        }

        private void btnNextImage_Click(object sender, RoutedEventArgs e)
        {
            if (currentShowTag == null)
            {
                if (images.Count == 0)
                    return;

                currentIndex = 0;
            }

            if (currentIndex < images.Count - 1)
            {
                currentIndex++;
                LoadPageShow();
            }
            else
            {
                // 可能要加载更多的数据了
                Load100ImageDatas();
                currentIndex = 0;
                btnNextImage_Click(null, null);
            }
        }

        void LoadPageShow()
        {
            currentShowTag = images[currentIndex];

            var image = Cv2.ImRead(currentShowTag.GetTrueImageFile());

            var cc = CascadeClassifierManager.Load("haarcascade_frontalface_alt2.xml");

            var tps = currentShowTag.Parts;

            if(tps == null || tps.Count == 0)
            {
                // 网上有看到有人先转灰度再识别，实际效果也没好多少 var gray_img = image.CvtColor(ColorConversionCodes.RGB2GRAY);
                var ract = cc.DetectMultiScale(image);

                tps = new List<TagPart>();

                foreach (var face in ract)
                {
                    var body = TagsDataSet.GetRect(face, (double) (64 - 8) / 128, image.Width, image.Height);

                    if (body != Rect.Empty)
                    {
                        tps.Add(new TagPart
                        {
                            Face = face,
                            Body = body
                        });
                    }
                }
            }

            if (tps.Count == 0)
            {
                UpdateIamgeTagStatus(currentShowTag, "opencv_fail");
                btnNextImage_Click(null, null);
                return;
            }

            currentShowTag.Parts = tps;

            panelParts.Children.Clear();

            foreach (var item in tps)
            {
                var face = item.Face;
                var body = item.Body;
                
                Cv2.Rectangle(image, face, Scalar.Red);

                var i2 = image.Clone(body);
                var utp = new UserTagParts();
                utp.InitData(item);
                utp.imgShow.Source = i2.MatToBitmapImage();
                panelParts.Children.Add(utp);

                Cv2.Rectangle(image, body, Scalar.Red);

                var ioa = InputOutputArray.Create(image);
                // 顺便标注一下尺寸
                Cv2.PutText(ioa, $"top:{face.Top} left:{face.Left} w:{face.Width} h:{face.Height}",
                    new Point(face.Left, face.Bottom + 2),
                    HersheyFonts.HersheySimplex, 1, Scalar.Blue);

                ioa = InputOutputArray.Create(image);
                Cv2.PutText(ioa, $"top:{body.Top} left:{body.Left} w:{body.Width} h:{body.Height}",
                    new Point(body.Left, body.Top - 10),
                    HersheyFonts.HersheySimplex, 1, Scalar.Blue);
            }

            imgShow.Source = image.MatToBitmapImage();

            panelCurrentImgTags.Children.Clear();

            if (!string.IsNullOrEmpty(currentShowTag.TagsName))
            {
                AppendTagtoCurrentTagPanel(currentShowTag.TagsName);
            }
        }

        private ImageTag currentShowTag = null;

        private readonly List<ImageTag> images = new List<ImageTag>();
        private int currentIndex = -1;

        private List<string> defaultTags = new List<string>();

        private void Window_Loaded(object sender, EventArgs e)
        {
            Load100ImageDatas();

            // LoadDefaultTagsToShow();

            btnNextImage_Click(null, null);
        }

        private void Load100ImageDatas()
        {
            images.Clear();

            // 窗口初始化结束
            using var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili);

            var list = db.Select<ImageTag>(o => o.Status == "downfile_finish").Take(100);

            foreach (var item in list)
            {
                var trueFile = item.GetTrueImageFile();

                if (File.Exists(trueFile))
                {
                    images.Add(item);
                }
            }
        }

        private void LoadDefaultTagsToShow()
        {
            //paneldefaultTags.Children.Clear();

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

            //paneldefaultTags.Children.Add(lab);
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
                currentShowTag.TagsName = exits.ToArray().ToCsv();

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
                //paneldefaultTags.Children.Remove(clickLabel);
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

        /// <summary>
        /// 保存当前的状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (currentShowTag != null)
            {
                var allTags = new HashSet<string>();

                foreach (var part in currentShowTag.Parts)
                {
                    if (part.TagNames == null)
                        continue;

                    foreach (var t in part.TagNames.Split(','))
                    {
                        allTags.Add(t);
                    }
                }

                currentShowTag.TagsName = allTags.ToArray().ToCsv();

                using var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili);
                currentShowTag.Status = "completed";
                db.Update(currentShowTag);

                btnNextImage_Click(null, null);
            }
        }

        private void btnOpenOutput_Click(object sender, RoutedEventArgs e)
        {
            using var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili);
            var tags = db.Select<ImageTag>(o => o.Status == "completed");
            TagsDataSet.WriteSourceData(tags, Path.Combine(Utils.DefaultDataFolder, "TrainData/bilibili.tags"));
            MessageBox.Show("输出完成！");
            // new Output().ShowDialog();
        }

        private void btnLater_Click(object sender, RoutedEventArgs e)
        {
            UpdateIamgeTagStatus(currentShowTag, "later");
            btnNextImage_Click(null, null);
        }

        private void UpdateIamgeTagStatus(ImageTag item, string status)
        {
            using var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili);
            item.Status = status;
            db.Update(currentShowTag);
        }

        /// <summary>
        /// 将当前图片设置为无效图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInvalidData_Click(object sender, RoutedEventArgs e)
        {
            UpdateIamgeTagStatus(currentShowTag, "invalid_data");
            btnNextImage_Click(null, null);
        }
    }
}