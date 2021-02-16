using BilibiliSpider.DB;
using BilibiliSpider.Entity.Database;
using BilibiliSpider.Spider.DataProcess;
using OpenCvSharp;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ExportClassifiedFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            // ExportImageFiles();

            string outFolder = @"V:\Data\ExportCalssifiedImage";
            string trainDataFolder = @"V:\Data\TrainData";
            CreateTrainData(outFolder, trainDataFolder);
        }

        /// <summary>
        /// 创建训练用的数据集
        /// </summary>
        /// <param name="imageFolder"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public unsafe static Result CreateTrainData(string imageFolder, string path, int width = (64 - 8),    int height = 128)
        {
            var imgFile = Path.Combine(path, "image.mat");

            Result result = new Result();

            var outFile = new FileInfo(imgFile);
            if (!outFile.Directory.Exists)
                outFile.Directory.Create();

            using var imgStream = outFile.OpenWrite();
            
            // 图片标签数量：int
            // 图片标签数组：图片数量*图片标签数量(多标签的处理,1表示带有对应的tag)
            // 图片标签名称：utf8字符串剩下的直接，出字符串后，用逗号分隔
            var labFile = Path.Combine(path, "lable.mat");
            using var labStream = File.OpenWrite(labFile);

            DirectoryInfo souceFolder = new DirectoryInfo(imageFolder);
            List<string> saveNames = new List<string>();

            int labIndex = 0;

            foreach(var dir in souceFolder.GetDirectories())
            {
                if (dir.Name == "")
                    continue;

                foreach(var imgFileName in dir.GetFiles("*.jpg"))
                {
                    var mat = Cv2.ImRead(imgFileName.FullName);
                    if (mat.GetArray<Vec3b>(out var arrVec3B))
                    {
                        var span = MemoryMarshal.CreateSpan<Vec3b>(ref arrVec3B[0], width * height);
                        var x = MemoryMarshal.AsBytes<Vec3b>(span);

                        imgStream.Write(x);

                        {
                            var buffer = new byte[1];
                            buffer[0] = (byte)labIndex;
                            labStream.Write(buffer);
                        }

                        result.PicNum++;
                        var labName = dir.Name;

                        if (result.LabelCount.ContainsKey(labName))
                        {
                            result.LabelCount[labName]++;
                        }
                        else
                        {
                            result.LabelCount[labName] = 1;
                        }
                    }
                }

                saveNames.Add(dir.Name);
                labIndex++;
            }

            var lines = new List<string>()
            {
                result.PicNum.ToString(),
                width.ToString(),
                height.ToString(),
            };

            lines.AddRange(saveNames);

            File.WriteAllLines(Path.Combine(path, "config.txt"), lines, new UTF8Encoding(false));

            return result;
        }

        /// <summary>
        /// 导出图片文件
        /// </summary>
        private static void ExportImageFiles()
        {
            using (var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili))
            {
                var querySQL = @"
SELECT ImageTag.* FROM 
ImageTag, AV
WHERE ImageTag.`Status` = 'opencv_finish' 
AND ImageTag.AvId = AV.Id
ORDER BY AV.ctime DESC
LIMIT 100
";
                var tags = db.Select<ImageTag>(querySQL);
                Console.WriteLine(tags.Count);
                var count = ExportImageFiles(@"V:\Data\ExportCalssifiedImage", tags);
                foreach (var item in tags)
                {
                    item.Status = "export";
                    db.Update(item);
                }

                Console.WriteLine(count);
            }
        }

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
        public unsafe static int ExportImageFiles(string outFolder, List<ImageTag> tags, int width = (64 - 8),
            int height = 128)
        {
            var di = new DirectoryInfo(outFolder);
            if (!di.Exists)
            {
                Console.WriteLine($"目录 {outFolder} 不存在!");
                return -1;
            }

            var map = new HashSet<string>();

            foreach(var fi in di.GetFiles("", SearchOption.AllDirectories))
            {
                map.Add(fi.Name);
            }

            int saveCount = 0;


            foreach (var item in tags)
            {
                var image = Cv2.ImRead(item.GetTrueImageFile());
                foreach (var part in item.OpenCvParts)
                {
                    // 将人物身体复制出来
                    var cutImage = image.Clone(part.Body);
                    var fileName = $"{item.AvId}_{part.Body.Top}_{part.Body.Left}_{part.Body.Bottom}_{part.Body.Right}.jpg";

                    if (map.Contains(fileName))
                        continue;

                    // 图片缩放
                    var okSize = cutImage.Resize(new Size(width, height));
                    fileName = Path.Combine(outFolder, fileName);
                    okSize.SaveImage(fileName);
                    saveCount++;
                }
            }

            return saveCount;
        }
    }
}
