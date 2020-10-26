using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BilibiliSpider.Common;
using BilibiliSpider.DB;
using BilibiliSpider.Entity.Database;
using DotnetSpider.DataFlow;
using DotnetSpider.Http;
using ServiceStack;
using ServiceStack.OrmLite;
using BilibiliSpider.Entity.Database;

namespace BilibiliSpider.Spider.DataProcess
{
    /// <summary>
    /// 视频的Tag标签获取
    /// </summary>
    class TagProcess : JsonPaser<TagProcess.Root>
    {
        #region Json
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Count
        {
            public int view { get; set; }
            public int use { get; set; }
            public int atten { get; set; }
        }

        public class Datum
        {
            public int tag_id { get; set; }
            public string tag_name { get; set; }
            public string cover { get; set; }
            public string head_cover { get; set; }
            public string content { get; set; }
            public string short_content { get; set; }
            public int type { get; set; }
            public int state { get; set; }
            public int ctime { get; set; }
            public Count count { get; set; }
            public int is_atten { get; set; }
            public int likes { get; set; }
            public int hates { get; set; }
            public int attribute { get; set; }
            public int liked { get; set; }
            public int hated { get; set; }
            public int extra_attr { get; set; }
            public string tag_type { get; set; }
            public bool is_activity { get; set; }
            public string color { get; set; }
            public int alpha { get; set; }
            public bool is_season { get; set; }
            public int subscribed_count { get; set; }
            public string archive_count { get; set; }
            public int featured_count { get; set; }
        }

        public class Root
        {
            public int code { get; set; }
            public string message { get; set; }
            public int ttl { get; set; }
            public List<Datum> data { get; set; }
        }



        #endregion

        public TagProcess():base("tag")
        {
        }
        public static Request CreateRquerst(int avId)
        {
            // https://api.bilibili.com/x/web-interface/view/detail/tag?aid=286927170

            var url = $"https://api.bilibili.com/x/web-interface/view/detail/tag?aid={avId}";
            var request = new Request()
            {
                RequestUri = new Uri(url)
            };

            request.Properties[REQUEST_CHECK_PROPERTY_NAME] = "tag";
            request.Properties["avId"] = avId;

            return request;
        }

        public override void OnHanlder(DataFlowContext context, Root parseObj)
        {
            if (parseObj.code != 0 && parseObj.data == null)
                return;

            var avId = (int)context.Request.Properties["avId"];

            using var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili);
            var up = db.SingleById<AV>(avId);

            var tags = parseObj.data;
            up.tagIds = tags.Select(o => o.tag_id).ToCsv();
            up.tagNames = tags.Select(o => o.tag_name).ToCsv();

            db.Update(up);

            // 单独处理 tag 标签数据

            foreach (var tag in tags)
            {
                var dbTag = db.SingleById<Tag>(tag.tag_id);

                if (dbTag == null)
                {
                    dbTag = new Tag {Id = tag.tag_id};
                    db.Insert(dbTag);
                }

                dbTag.name = tag.tag_name;
                dbTag.ctime = tag.ctime;
                dbTag.cover = tag.cover;
                dbTag.head_cover = tag.head_cover;
                dbTag.subscribed_count = tag.subscribed_count;
                dbTag.archive_count = tag.archive_count;
                dbTag.featured_count = tag.featured_count;
                dbTag.alpha = tag.alpha;
                dbTag.color = tag.color;
                dbTag.tag_type = tag.tag_type;
                dbTag.content = tag.content;
                dbTag.short_content = tag.short_content;
                dbTag.use = tag.count.use;

                db.Update(dbTag);
            }
        }
    }
}
