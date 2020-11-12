using System;
using System.IO;
using System.Runtime.InteropServices;
using BilibiliSpider.Common;
using OpenCvSharp;

namespace TrainingAI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ReadTrainData();
        }

        /// <summary>
        /// 读取训练用的数据
        /// </summary>
        unsafe static void ReadTrainData()
        {
            var filePath = Path.Combine(Utils.DefaultDataFolder, "TrainData/bilibili.tags");

            var stream = File.OpenRead(filePath);
            byte[] intBuffer = new byte[4];

            stream.Read(intBuffer);
            var picNum = BitConverter.ToInt32(intBuffer);

            stream.Read(intBuffer);
            var width = BitConverter.ToInt32(intBuffer);

            stream.Read(intBuffer);
            var height = BitConverter.ToInt32(intBuffer);

            stream.Read(intBuffer);
            var channels = BitConverter.ToInt32(intBuffer);

            var buffer = new byte[width * height * channels];
            var v3 = MemoryMarshal.AsRef<Vec3b>(buffer);

            for (var i = 0; i < picNum; i++)
            {
                stream.Read(buffer);
                var mat = new Mat(height, width, MatType.CV_8UC3, buffer);

                // mat.SaveImage($"r:/t/{i}.png"); 输出图片demo
            }

        }
    }
}
