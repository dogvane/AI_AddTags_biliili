using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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

            // 图片标签数量：int
            // 图片标签数组：图片数量*图片标签数量(多标签的处理,1表示带有对应的tag)
            // 图片标签名称：utf8字符串剩下的直接，出字符串后，用逗号分隔

            stream.Read(intBuffer);
            var tagNum = BitConverter.ToInt32(intBuffer);

            List<byte[]> imageTags = new List<byte[]>();

            for (var i = 0; i < picNum; i++)
            {
                var tagbuffer = new byte[tagNum];
                stream.Read(tagbuffer);
                imageTags.Add(tagbuffer);
            }

            var lessLen = stream.Length - stream.Position;
            var strBuffer = new byte[lessLen];
            stream.Read(strBuffer);
            string names = Encoding.UTF8.GetString(strBuffer);
            Console.WriteLine(names);
        }
    }
}
