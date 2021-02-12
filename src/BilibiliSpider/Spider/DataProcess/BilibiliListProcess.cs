using System;
using BilibiliSpider.Common;
using BilibiliSpider.Spider.DataProcess;
using BilibiliSpider.DB;
using BilibiliSpider.Entity.Database;
using BilibiliSpider.Entity.Spider;
using DotnetSpider.DataFlow;
using DotnetSpider.Http;
using ServiceStack.OrmLite;

namespace BilibiliSpider.Spider
{
    public class BilibiliListProcess : JsonPaser<BilibiliListProcess.BilibiliListRet>
    {

        #region json entity
        public class BilibiliListRet
        {
            public int code { get; set; }

            public string message { get; set; }

            public Data data { get; set; }

            public class Data
            {
                public Archives[] archives { get; set; }
                public Page page { get; set; }
            }

            public class Page
            {
                public int page { get; set; }
                public int num { get; set; }
                public int size { get; set; }

            }
            public class Archives
            {
                public int aid { get; set; }
                public int videos { get; set; }
                public string bvid { get; set; }

                /// <summary>
                /// 板块id，好像名字换了
                /// </summary>
                public int tid { get; set; }
                public string tname { get; set; }
                public int copyright { get; set; }
                public string pic { get; set; }
                public string title { get; set; }
                public int pubdate { get; set; }
                public int ctime { get; set; }
                public string desc { get; set; }
                public int state { get; set; }
                public int attribute { get; set; }
                public int duration { get; set; }
                public int mission_id { get; set; }
                public Rights rights { get; set; }
                public Owner owner { get; set; }
                public Stat stat { get; set; }
                public string dynamic { get; set; }
                public int cid { get; set; }
                public Dimension dimension { get; set; }
            }
        }

        #endregion

        public BilibiliListProcess():base("list")
        {
        }

        int notfindCount = 0;

        public override void OnHanlder(DataFlowContext context, BilibiliListRet parseObj)
        {
            // Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(parseObj));

            if (parseObj == null)
                return;

            if (parseObj.data.archives.Length == 0)
                return;

            using var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili);

            int newCount = 0;

            foreach (var item in parseObj.data.archives)
            {
                var av = new AV()
                {
                    Id = item.aid,
                    bvId = item.bvid,
                    copyright = item.copyright,
                    ctime = item.ctime,
                    ctime2 = item.ctime.UnixToDateTime().ToLongDateString(),
                    pic = item.pic,
                    title = item.title,
                    videos = item.videos,
                    view = item.stat.view,
                    rid = item.tid,
                    UpId = item.owner.mid,
                    cid = item.cid,
                };

                av.stat = item.stat;
                var existsDB = db.SingleById<AV>(av.Id);

                if (existsDB != null)
                {
                    // 之前有过这个视频，则忽略这次操作
                    Console.WriteLine($"exits {existsDB.title}");
                    continue;
                }

                db.Insert(av);
                newCount++;

                // 这里可以获得up的一些简单的信息 https://api.bilibili.com/x/web-interface/card?mid=3630684&photo=1
                // 这里不判断了，只要发新视频，都更新一次up主信息
                var request = UpProcess.CreateRquerst(av.UpId);
                context.AddFollowRequests(request);

                // 爬取 tag 信息，这个信息可能需要不断的更新才行，但也仅限于视频更新1个月以内吧 https://api.bilibili.com/x/web-interface/view/detail/tag?aid=286927170
                request = TagProcess.CreateRquerst(av.Id);
                context.AddFollowRequests(request);

                // todo 如果视频有多个，还得获得视频下面分视频的数据 https://api.bilibili.com/x/player/pagelist?bvid=BV1wf4y1X7ka

                // 获得视频的简介 https://api.bilibili.com/x/web-interface/archive/desc?aid=286927170
                request = DescProcess.CreateRquerst(av.Id);
                context.AddFollowRequests(request);

                // todo 定时更新获得视频的状态 https://api.bilibili.com/x/web-interface/archive/stat?aid=286927170

                // todo 读取评论信息 https://api.bilibili.com/x/v2/reply?pn=2&type=1&oid=286927170&sort=0
                //  评论的回复翻页内容 https://api.bilibili.com/x/v2/reply/reply?&pn=2&type=1&oid=244913305&ps=10&root=3602777175

                // todo 抓取弹幕信息 http://comment.bilibili.com/245666614.xml 这里用的是cid

                // 爬取封面照片
                request = ImageProcess.CreateRequest(av);
                context.AddFollowRequests(request);
            }

            if(newCount > -1)
            {
                notfindCount++;
            }
            else
            {
                notfindCount = 0;
            }


            if (notfindCount < 20)
            {
                Console.WriteLine("getNextPage");
                var page = (int)context.Request.Properties["pageNo"];
                var tid = (int) context.Request.Properties["rid"];

                // if (page < 2)
                {
                    var request = CreateListRequest(tid, page + 1);
                    context.AddFollowRequests(request);
                }
            }
            //else
            //{
            //    Console.WriteLine("finish");
            //}
        }

        /// <summary>
        /// b站的翻页最大50条
        /// </summary>
        public const int pageSize = 50;

        /// <summary>
        /// 创建一个查询请求
        /// 只通过页码来处理
        /// </summary>
        /// <param name="rid">板块id</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        public static Request CreateListRequest(int rid, int page)
        {
            // https://api.bilibili.com/x/web-interface/newlist?rid=122&type=0&pn=2&ps=20
            var url = $"https://api.bilibili.com/x/web-interface/newlist?rid={rid}&type=0&pn={page}&ps={pageSize}";
            var request = new Request()
            {
                RequestUri = new Uri(url)
            };

            request.Properties[REQUEST_CHECK_PROPERTY_NAME] = "list";
            request.Properties["pageNo"] = page;
            request.Properties["rid"] = rid;

            return request;
        }
    }
}
