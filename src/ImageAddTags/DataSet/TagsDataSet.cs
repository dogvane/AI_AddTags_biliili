using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using BilibiliSpider.Entity.Database;
using BilibiliSpider.Spider.DataProcess;
using OpenCvSharp;
using ServiceStack;
using Newtonsoft.Json;
using NumSharp;
using Tensorflow.Keras.Engine;
using BilibiliSpider.Common;
using AITag;
using Tensorflow;
using Tensorflow.Keras;
using Tensorflow.Keras.Layers;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;
using Utils = BilibiliSpider.Common.Utils;

namespace ImageAddTags.DataSet
{
    class TagsDataSet
    {
        public class Result
        {
            /// <summary>
            /// 图片数量
            /// </summary>
            public int PicNum { get; set; }

            public Dictionary<string, int> LabelCount { get; set; } = new Dictionary<string, int>();
        }

        /// <summary>
        /// 这个写数据的，不是将所有数据写到一个文件里，而是写入多个文件里
        /// 
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="path"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public unsafe static Result WriteSourceData2(List<ImageTag> tags, string path, int width = (64 - 8),
            int height = 128)
        {
            var imgFile = Path.Combine(path, "image.mat");
            
            Result result = new Result();

            var outFile = new FileInfo(imgFile);
            if (!outFile.Directory.Exists)
                outFile.Directory.Create();

            using var stream = outFile.OpenWrite();

            List<TagPart> saveParts = new List<TagPart>();  // 参与保存的图片信息
            HashSet<string> mapNames = new HashSet<string>();
            int index2 = 0;

            List<object> sourceList = new List<object>();

            foreach (var item in tags)
            {
                var image = Cv2.ImRead(item.GetTrueImageFile());
                foreach (var part in item.OpenCvParts)
                {
                    if (string.IsNullOrEmpty(part.TagNames) ||
                        part.TagNames.Contains("无效"))
                    {
                        continue;
                    }

                    // 将人物身体复制出来
                    var cutImage = image.Clone(part.Body);
                    // 图片缩放
                    var okSize = cutImage.Resize(new Size(width, height));
                    
                    if (okSize.GetArray<Vec3b>(out var arrVec3B))
                    {
                        // 字节流是BGR模式的

                        var span = MemoryMarshal.CreateSpan<Vec3b>(ref arrVec3B[0], width * height);
                        var x = MemoryMarshal.AsBytes<Vec3b>(span);

                        okSize.SaveImage($"r:/{index2++}.png");

                        stream.Write(x);
                        saveParts.Add(part);

                        var names = part.TagNames.Split(',');
                        foreach (var n in names)
                            mapNames.Add(n);

                        result.PicNum++;
                        var key = names[0];

                        if (result.LabelCount.ContainsKey(key))
                        {
                            result.LabelCount[key]++;
                        }
                        else
                        {
                            result.LabelCount[key] = 1;
                        }
                    }
                }
            }
            stream.Flush();
            stream.Close();


            // 图片标签数量：int
            // 图片标签数组：图片数量*图片标签数量(多标签的处理,1表示带有对应的tag)
            // 图片标签名称：utf8字符串剩下的直接，出字符串后，用逗号分隔
            var labFile = Path.Combine(path, "lable.mat");
            using var labStream = File.OpenWrite(labFile);

            using var labStream2 = File.OpenWrite(Path.Combine(path, "lable2.mat"));    // 多标签的数据输出

            var saveNames = mapNames.OrderBy(o => o).ToList();

            foreach (var part in saveParts)
            {
                {
                    var buffer = new byte[saveNames.Count];

                    foreach (var n in part.TagNames.Split(','))
                    {
                        var index = saveNames.IndexOf(n);
                        buffer[index] = 1;
                    }

                    labStream2.Write(buffer);
                }

                {
                    var index = saveNames.IndexOf(part.TagNames.Split(',')[0]);
                    var buffer = new byte[1];
                    buffer[0] = (byte)index;
                    labStream.Write(buffer);
                }
            }

            var lines = new List<string>()
            {
                saveParts.Count.ToString(),
                width.ToString(),
                height.ToString(),
            };

            lines.AddRange(saveNames);

            File.WriteAllLines(Path.Combine(path, "config.txt"), lines, new UTF8Encoding(false));

            return result;
        }

        public unsafe static void WriteSourceData(List<ImageTag> tags, string fileName, int width = (64 - 8), int height = 128)
        {
            // 简单一点，文件的格式是
            // 图片数量: int
            // 图片宽度: int
            // 图片高度: int
            // 图片通道数量: int （默认需要3通道）
            // 图片byte数据，长度为 图片数量*图片宽度*图片高度*图片通道数量， 如果长度不满足，说明数据有问题。
            // 图片标签数量：int
            // 图片标签数组：图片数量*图片标签数量(多标签的处理,1表示带有对应的tag)
            // 图片标签名称：utf8字符串剩下的直接，出字符串后，用逗号分隔

            var outFile = new FileInfo(fileName);
            if (!outFile.Directory.Exists)
                outFile.Directory.Create();

            using var stream = outFile.OpenWrite();

            stream.Write(BitConverter.GetBytes(tags.Count));
            stream.Write(BitConverter.GetBytes(width));
            stream.Write(BitConverter.GetBytes(height));
            stream.Write(BitConverter.GetBytes((int)3)); // 通道数量

            List<TagPart> saveParts = new List<TagPart>();  // 参与保存的图片信息
            HashSet<string> mapNames = new HashSet<string>();

            foreach (var item in tags)
            {
                var image = Cv2.ImRead(item.GetTrueImageFile());
                foreach (var part in item.OpenCvParts)
                {
                    if (string.IsNullOrEmpty(part.TagNames) || 
                        part.TagNames.Contains("无效"))
                    {
                        continue;
                    }

                    var cutImage = image.Clone(part.Body);
                    // 图片缩放
                    var okSize = cutImage.Resize(new Size(width, height));

                    if (okSize.GetArray<Vec3b>(out var arrVec3B))
                    {
                        var span = MemoryMarshal.CreateSpan<Vec3b>(ref arrVec3B[0], width * height);
                        var x = MemoryMarshal.AsBytes<Vec3b>(span);

                        // var s2 = new Mat(height, width, MatType.CV_8UC3, x.ToArray());
                        // s2.SaveImage($"r:/t/{i++}.png"); 输出图片demo

                        stream.Write(x);
                        saveParts.Add(part);

                        var names = part.TagNames.Split(',');
                        foreach (var n in names)
                            mapNames.Add(n);
                    }
                }
            }

            // 图片标签数量：int
            // 图片标签数组：图片数量*图片标签数量(多标签的处理,1表示带有对应的tag)
            // 图片标签名称：utf8字符串剩下的直接，出字符串后，用逗号分隔

            stream.Write(BitConverter.GetBytes(mapNames.Count));
            var saveNames = mapNames.OrderBy(o => o).ToList();

            foreach (var part in saveParts)
            {
                var buffer = new byte[saveNames.Count];

                foreach (var n in part.TagNames.Split(','))
                {
                    var index = saveNames.IndexOf(n);
                    buffer[index] = 1;
                }

                stream.Write(buffer);
            }

            var tagBuffer = Encoding.UTF8.GetBytes(saveNames.ToCsv());
            stream.Write(tagBuffer);
            
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(saveParts.Count));

            stream.Flush();
            stream.Close();
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        public static BilibiliDataSet ReadDataSets(string basePath)
        {
            var configFileName = Path.Combine(basePath, "config.txt");

            var configLines = File.ReadAllLines(configFileName);

            var ret = new BilibiliDataSet();
            ret.Num = int.Parse(configLines[0]);
            ret.Width = int.Parse(configLines[1]);
            ret.Height = int.Parse(configLines[2]);

            ret.LabNames = configLines[3..];

            var imageFileName = Path.Combine(basePath,  "image.mat");

            var bytes = File.ReadAllBytes(imageFileName);

            var images = np.array(bytes) / 255.0f;
            ret.Images = images.reshape(ret.Num, ret.Height, ret.Width, 3);

            var singleLabelFileName = Path.Combine(basePath, "lable.mat");

            var labBytes = File.ReadAllBytes(singleLabelFileName);
            ret.Lables = np.array(labBytes).reshape(ret.Num, 1);

            return ret;
        }


        static Functional s_model;
        static BilibiliDataSet s_dataSet;

        public static Functional GetModelInstance()
        {
            if (s_model == null)
            {
                var folderName = Path.Combine(Utils.DefaultDataFolder, "TrainData/");

                s_dataSet = ReadDataSets(folderName);

                s_model = TFModels.GetBilibiliModelV1(s_dataSet.Width, s_dataSet.Height, s_dataSet.LabNames.Length);
                
                var weightsFileName = Path.Combine(Utils.DefaultDataFolder, "TrainData/bilibili.h5");
                if(File.Exists(weightsFileName))
                    s_model.load_weights(weightsFileName);
            }

            return s_model;
        }

        public static string ModelTest(Mat mat)
        {
            var model = GetModelInstance();

            var width = s_dataSet.Width;
            var height = s_dataSet.Height;

            if(mat.Width != width  && mat.Height != height)
            {
                mat = mat.Resize(new Size(width, height));
            }

            if (mat.GetArray<Vec3b>(out var arrVec3B))
            {
                // 字节流是BGR模式的

                var span = MemoryMarshal.CreateSpan<Vec3b>(ref arrVec3B[0], width * height);
                var x = MemoryMarshal.AsBytes<Vec3b>(span);

                var ts = np.array(x.ToArray()) / 255.0f; ;
                ts = ts.reshape(1, height, width, 3);

                var result = model.predict(ts, 1);
                var index = tf.argmax(result[0]).numpy()[0];
                return s_dataSet.LabNames[index];
            }

            return string.Empty;
        }

        public static void ChangeModel(Functional model, BilibiliDataSet dataSet)
        {
            s_model = model;
            s_dataSet = dataSet;
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
