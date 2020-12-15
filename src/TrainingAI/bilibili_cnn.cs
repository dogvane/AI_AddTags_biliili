using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NumSharp;

namespace TrainingAI
{
    /// <summary>
    /// b站的标签识别
    /// 目前还只是单标签的处理
    /// </summary>
    public class bilibili_cnn
    {
        /// <summary>
        /// 读取数据
        /// </summary>
        public BilibiliDataSet ReadDataSets()
        {
            var configLines = File.ReadAllLines("V:/Data/TrainData/config.txt");

            var ret = new BilibiliDataSet();
            ret.Num = int.Parse(configLines[0]);
            ret.Width = int.Parse(configLines[1]);
            ret.Height = int.Parse(configLines[2]);

            ret.LabNames = configLines[3..];

            var imageFileName = "V:/Data/TrainData/image.mat";

            var bytes = File.ReadAllBytes(imageFileName);

            var images = np.array(bytes) / 255.0f;
            ret.Images = images.reshape(ret.Num, ret.Height, ret.Width, 3);

            var singleLabelFileName = "V:/Data/TrainData/lable.mat";

            var labBytes = File.ReadAllBytes(singleLabelFileName);
            ret.Lables = np.array(labBytes).reshape(ret.Num, 1);

            return ret;
        }



        /// <summary>
        /// 训练数据模型
        /// </summary>
        public void TrainModule()
        {

        }


    }

    public class BilibiliDataSet
    {
        public int Num { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string[] LabNames { get; set; }

        [JsonIgnore]
        public NDArray Images;

        /// <summary>
        /// 单标签数据
        /// </summary>
        [JsonIgnore]
        public NDArray Lables;
    }
}
