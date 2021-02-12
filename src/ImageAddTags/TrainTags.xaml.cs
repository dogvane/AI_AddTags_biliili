using BilibiliSpider.Common;
using BilibiliSpider.DB;
using BilibiliSpider.Entity.Database;
using ImageAddTags.DataSet;
using ServiceStack.OrmLite;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AITag;
using Path = System.IO.Path;

namespace ImageAddTags
{
    /// <summary>
    /// TrainTags.xaml 的交互逻辑
    /// </summary>
    public partial class TrainTags : Window
    {
        public TrainTags()
        {
            InitializeComponent();
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            if (isSaveWeights)
            {
                this.Close();
                return;
            }

            isRuning = false;
            isSaveWeights = false;
            labStatus.Content = "等待保存中...";

            ThreadPool.QueueUserWorkItem(o =>  
            {
                while (!isSaveWeights)
                {
                    Thread.Sleep(500);
                }

                this.Dispatcher.Invoke(() =>
                {
                    this.Close();
                });

                if (thread.ThreadState == ThreadState.Running)
                    thread.Abort();
            });
        }

        bool isSaveWeights = false;
        bool isRuning = false;

        void TreadTrain()
        {
            this.Dispatcher.Invoke(() => { 
                labStatus.Content = "保存训练数据中...";
            });

            using var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili);
            var tags = db.Select<ImageTag>(o => o.Status == "completed");

            var folder = Path.Combine(Utils.DefaultDataFolder, "TrainData/");
            var saveResult = TagsDataSet.WriteSourceData2(tags, folder);

            this.Dispatcher.Invoke(() => {
                labStatus.Content = "开始加载训练数据";
            });

            var dataSet = TagsDataSet.ReadDataSets(folder);

            var model = TFModels.GetBilibiliModelV1(dataSet.Width, dataSet.Height, dataSet.LabNames.Length);
            var weightsFileName = Path.Combine(folder, "bilibili.h5");
            //if (File.Exists(weightsFileName))
            //{
            //    model.load_weights(weightsFileName);
            //}

            isRuning = true;
            var epochs = 0;
            var epochsStep = 5;

            this.Dispatcher.Invoke(() => {
                labStatus.Content = "开始训练";
                lab_pic.Content = $"训练用图片：{saveResult.PicNum}";
                lab_label.Content = string.Join("\n", saveResult.LabelCount.OrderByDescending(o => o.Value).Select(o => $"{o.Key}({o.Value}) "));
            });

            DateTime start = DateTime.Now;

            while (isRuning)
            {
                model.fit(dataSet.Images, dataSet.Lables, epochs: epochsStep);
                var metrices = model.metrics.ToArray();
                var loss = (float)metrices[0].result();
                var accuracy = (float)metrices[1].result();

                epochs += epochsStep;
                this.Dispatcher.Invoke(() =>
                {
                    labStatus.Content = $"训练中: {epochs} 轮 用时:{(DateTime.Now - start)}";

                    labLoss.Content = $"损失率: {loss * 100}";
                    labAccuracy.Content = $"准确率: {accuracy * 100}";
                    pb.Value = accuracy * 100;
                });

                if (accuracy > 0.95 && loss < 0.1)
                {
                    break;
                }
            }

            model.save_weights(weightsFileName);
            
            TagsDataSet.ChangeModel(model, dataSet);

            isSaveWeights = true;

            this.Dispatcher.Invoke(() => {
                btnQuit.Content = "退出";
            });
        }

        Thread thread = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 窗口加载成功，先进入后台保存阶段，然后线程进入训练阶段
            thread = new Thread(TreadTrain);
            thread.Start();
        }
    }
}
