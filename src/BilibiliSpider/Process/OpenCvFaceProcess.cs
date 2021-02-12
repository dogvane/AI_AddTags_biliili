using BilibiliSpider.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.OrmLite;
using BilibiliSpider.Entity.Database;
using BilibiliSpider.Spider.DataProcess;
using OpenCvSharp;
using AITag;

namespace BilibiliSpider.Process
{
    /// <summary>
    /// 用opencv对数据做处理
    /// </summary>
    class OpenCvFaceProcess
    {
        public static void Do()
        {
            var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili);
            foreach(var imgTag in db.Select<ImageTag>(o => o.Status == "downfile_finish"))
            {
                ImageProcess.OpenCVAnalysis(imgTag);
                db.Update(imgTag);
                GC.Collect();
            }
        }
    }
}
