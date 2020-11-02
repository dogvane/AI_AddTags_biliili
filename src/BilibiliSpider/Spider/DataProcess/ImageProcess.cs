using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using BilibiliSpider.Entity.Database;
using BilibiliSpider.Entity.Database;
using DotnetSpider.Http;
using System.IO;
using System.Threading.Tasks;
using BilibiliSpider.Common;
using BilibiliSpider.DB;
using DotnetSpider.DataFlow;
using DotnetSpider.DataFlow.Parser;
using ServiceStack.OrmLite;

namespace BilibiliSpider.Spider.DataProcess
{
    /// <summary>
    /// 图片处理流程
    /// </summary>
    public class ImageProcess: DataParser
    {
        /// <summary>
        /// 根据一个AV，创建一个可以用来图片下载策略
        /// </summary>
        /// <param name="av"></param>
        /// <returns></returns>
        public static Request CreateRequest(AV av)
        {
            var img = new ImageTag()
            {
                ImageUrl = av.pic,
                AvId = av.Id,
                UpId = av.UpId,
                LocalFileName = GetLocalFile(av.pic)
            };

            var trueFile = Path.Combine(DefaultImagePath, img.LocalFileName);

            if (File.Exists(trueFile))
            {
                // 本地下载过文件，就不考虑再下载图片数据了
                return null;
            }

            var ret = new Request
            {
                RequestUri = new Uri(img.ImageUrl),
            };

            ret.Properties["requestType"] = "image";
            ret.Properties[typeof(ImageTag).Name] = img;

            return ret;
        }

        static ImageProcess()
        {
            DefaultPath = Utils.DefaultDataFolder;
            DefaultImagePath = Path.Combine(Utils.DefaultDataFolder, "Images/");
        }

        public static string DefaultPath = string.Empty;

        public static string DefaultImagePath = string.Empty;

        private static string GetLocalFile(string pic)
        {
            // http://i1.hdslb.com/bfs/archive/62e9f4346360e0522ae1f7f38166562caeb6a8cb.jpg

            var uri = new Uri(pic);

            return uri.Host.Replace(".", "_") + uri.LocalPath;
        }

        /// <summary>
        /// 获得真正用于文件读取的路径
        /// </summary>
        /// <param name="imageLoccalFile"></param>
        /// <returns></returns>
        public static string GetTrueFile(string imageLoccalFile)
        {
            return Path.Combine(DefaultImagePath, imageLoccalFile);
        }

        protected override async Task ParseAsync(DataFlowContext context)
        {
            object type;
            if (!context.Request.Properties.TryGetValue("requestType", out type))
                return;

            if (type as string != "image")
                return;

            var image = context.Request.Properties[typeof(ImageTag).Name] as ImageTag;
            if (image == null)
                return;

            var imageBytes = context.Response.Content.Bytes;
            var trueFile = Path.Combine(DefaultImagePath, image.LocalFileName);

            var fi = new FileInfo(trueFile);
            if(!fi.Directory.Exists)
                fi.Directory.Create();

            File.WriteAllBytes(trueFile, imageBytes);

            using var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili);
            var dbItem = db.Single<ImageTag>(o => o.ImageUrl == image.ImageUrl);

            if (dbItem == null)
            {
                image.Status = "downfile_finish";
                db.Insert(image);
            }
            else
            {
                dbItem.Status = "downfile_finish";
                db.Update(dbItem);
            }

        }
    }

    public static class ImageProcessExtend
    {
        public static string GetTrueImageFile(this ImageTag image)
        {
            return Path.Combine(ImageProcess.DefaultImagePath, image.LocalFileName);
        }
    }
}
