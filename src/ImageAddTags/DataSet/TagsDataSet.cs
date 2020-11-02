using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BilibiliSpider.Entity.Database;
using BilibiliSpider.Spider.DataProcess;
using OpenCvSharp;

namespace ImageAddTags.DataSet
{
    class TagsDataSet
    {
        public static void WriteSourceData(List<ImageTag> tags, string fileName, int width = 64, int height = 128)
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

            var cc = CascadeClassifierManager.Load("haarcascade_frontalface_alt2.xml");

            foreach (var item in tags)
            {
                var image = Cv2.ImRead(item.GetTrueImageFile());

                var ract = cc.DetectMultiScale(image);

                if (ract.Length > 0)
                {
                    foreach (var r in ract)
                    {
                        // 根据脸部区域，裁切出带人物的尺寸大小
                        var middle = new {x = r.Top + r.Height / 2, y = r.Left + r.Width / 2};


                        Cv2.Rectangle(image, r, Scalar.Red);
                        // 顺便标注一下尺寸
                        var ioa = InputOutputArray.Create(image);
                        Cv2.PutText(ioa, $"w:{r.Width} h:{r.Height}", new Point(r.Left, r.Bottom + 2),
                            HersheyFonts.HersheyDuplex, 1, Scalar.Blue);
                    }
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="faceRect"></param>
        /// <param name="rate">宽高比 w/h </param>
        /// <param name="picWidth">图片的宽度</param>
        /// <param name="picHeight">图片高度</param>
        /// <returns></returns>
        public static Rect GetRect(Rect faceRect, double rate, int picWidth, int picHeight)
        {
            // 中心点
            var middle = new {x = faceRect.Left + faceRect.Width / 2, y = faceRect.Top + faceRect.Height / 2};
            
            // 脸的大小（中点的半径)
            var r = Math.Min(faceRect.Height, faceRect.Width) / 2;

            var height = r * 17; // 八头身
            var width = (int)(height * rate); // 截取长宽比是 1:2

            var left = middle.x - (int)width / 2;
            var top = middle.y - r * 2;

            var ret = new Rect(left, top, width, height);

            // 判断body是否能满足截取需求

            if (ret.Top > 0 &&
                ret.Left > 0 &&
                ret.Top + height < picHeight &&
                ret.Left + width < picWidth)
            {
                // 矩形刚好再图片范围内
                return ret;
            }

            // 如果不在范围内，则尝试缩小一下范围，看看能不能放下
            var wr = 0.0;
            var hr = 0.0;

            if (ret.Top < 0)
            {
                hr = (double)-ret.Top / height;
            }

            if (ret.Bottom > picHeight)
            {
                hr = (double) (ret.Bottom - picHeight) / height;
            }

            if (ret.Left < 0)
            {
                wr = (double) -ret.Left / height;
            }

            if (ret.Right > picWidth)
            {
                wr = (double)(ret.Right - picWidth) / width;
            }

            var maxR = Math.Max(hr, wr);

            if (maxR < 0.2)
            {
                // 变动不超过2层，可以进行截取操作，否则先无视
                var nHeight = (int)(height * (1 - maxR));
                var nWidth = (int)(height * rate);
                var ret2 = new Rect( ret.X + ((width - nWidth) / 2), ret.Y + Math.Min(((height - nHeight) / 2), r), nWidth, nHeight);
                Console.WriteLine($"{ret} -> {ret2}");

                return ret2;
            }

            return Rect.Empty;
        }
    }
}
