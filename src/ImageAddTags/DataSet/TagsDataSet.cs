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

namespace ImageAddTags.DataSet
{
    class TagsDataSet
    {
        /// <summary>
        /// 这个写数据的，不是将所有数据写到一个文件里，而是写入多个文件里
        /// 
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="path"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public unsafe static void WriteSourceData2(List<ImageTag> tags, string path, int width = (64 - 8),
            int height = 128)
        {
            var imgFile = Path.Combine(path, "image.mat");

            var outFile = new FileInfo(imgFile);
            if (!outFile.Directory.Exists)
                outFile.Directory.Create();

            using var stream = outFile.OpenWrite();

            List<TagPart> saveParts = new List<TagPart>();  // 参与保存的图片信息
            HashSet<string> mapNames = new HashSet<string>();
            int index2 = 0;

            foreach (var item in tags)
            {
                var image = Cv2.ImRead(item.GetTrueImageFile());
                foreach (var part in item.Parts)
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
                foreach (var part in item.Parts)
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
                hr = (double)-(ret.Top*2) / height;
            }

            if (ret.Bottom > picHeight)
            {
                hr = (double) ((ret.Bottom - picHeight)*2) / height;
            }

            if (ret.Left < 0)
            {
                wr = (double) -(ret.Left*2) / height;
            }

            if (ret.Right > picWidth)
            {
                wr = (double)((ret.Right - picWidth) *2) / width;
            }

            var maxR = Math.Max(hr, wr);

            if (maxR < 0.3)
            {
                // 变动不超过2层，可以进行截取操作，否则先无视
                var nHeight = (int)(height * (1 - maxR));
                var nWidth = (int)(height * rate);
                var ret2 = new Rect(ret.X + ((width - nWidth) / 2), ret.Y + Math.Min(((height - nHeight) / 2), r), nWidth, nHeight);
                Console.WriteLine($"{ret} -> {ret2}");

                if (ret2.Top > 0 &&
                    ret2.Left > 0 &&
                    ret2.Top + height < picHeight &&
                    ret2.Left + width < picWidth)
                {
                    // 矩形刚好再图片范围内
                    return ret2;
                }
            }

            return Rect.Empty;
        }
    }
}
