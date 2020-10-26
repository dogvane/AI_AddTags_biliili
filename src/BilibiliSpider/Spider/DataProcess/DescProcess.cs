using System;
using System.Collections.Generic;
using System.Text;
using BilibiliSpider.Common;
using BilibiliSpider.DB;
using BilibiliSpider.Entity.Database;
using DotnetSpider.DataFlow;
using DotnetSpider.Http;
using ServiceStack.OrmLite;

namespace BilibiliSpider.Spider.DataProcess
{
    /// <summary>
    /// 视频的简介，好像是要单独获取的
    /// </summary>
    class DescProcess : JsonPaser<DescProcess.Root>
    {
        #region Json
        public class Root
        {
            public int code { get; set; }
            public string message { get; set; }
            public int ttl { get; set; }
            public string data { get; set; }
        }

        #endregion

        public DescProcess():base("desc")
        {
            
        }

        public static Request CreateRquerst(int avId)
        {
            // https://api.bilibili.com/x/web-interface/archive/desc?aid=286927170

            var url = $"https://api.bilibili.com/x/web-interface/archive/desc?aid={avId}";
            var request = new Request()
            {
                RequestUri = new Uri(url)
            };

            request.Properties[REQUEST_CHECK_PROPERTY_NAME] = "desc";
            request.Properties["avId"] = avId;

            return request;
        }

        public override void OnHanlder(DataFlowContext context, Root parseObj)
        {
            if (parseObj.code != 0 && !string.IsNullOrEmpty(parseObj.data))
                return;

            var avId = (int)context.Request.Properties["avId"];

            using var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili);
            var up = db.SingleById<AV>(avId);

            up.desc = parseObj.data;
            
            db.Update(up);
        }
    }
}
