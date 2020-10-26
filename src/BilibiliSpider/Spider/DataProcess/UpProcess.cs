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
    class UpProcess : JsonPaser<UpProcess.Root>
    {
        #region Json

        public class LevelInfo
        {
            public int current_level { get; set; }
            public int current_min { get; set; }
            public int current_exp { get; set; }
            public int next_exp { get; set; }
        }

        public class Pendant
        {
            public int pid { get; set; }
            public string name { get; set; }
            public string image { get; set; }
            public int expire { get; set; }
            public string image_enhance { get; set; }
        }


        public class Official
        {
            public int role { get; set; }
            public string title { get; set; }
            public string desc { get; set; }
            public int type { get; set; }
        }

        public class OfficialVerify
        {
            public int type { get; set; }
            public string desc { get; set; }
        }

        public class Vip
        {
            public int vipType { get; set; }
            public string dueRemark { get; set; }
            public int accessStatus { get; set; }
            public int vipStatus { get; set; }
            public string vipStatusWarn { get; set; }
            public int theme_type { get; set; }
        }

        public class Card
        {
            public string mid { get; set; }
            public string name { get; set; }
            public bool approve { get; set; }
            public string sex { get; set; }
            public string rank { get; set; }
            public string face { get; set; }
            public string DisplayRank { get; set; }
            public int regtime { get; set; }
            public int spacesta { get; set; }
            public string birthday { get; set; }
            public string place { get; set; }
            public string description { get; set; }
            public int article { get; set; }
            public List<object> attentions { get; set; }
            public int fans { get; set; }
            public int friend { get; set; }
            public int attention { get; set; }
            public string sign { get; set; }
            public LevelInfo level_info { get; set; }
            public Pendant pendant { get; set; }
            public Nameplate nameplate { get; set; }
            public Official Official { get; set; }
            public OfficialVerify official_verify { get; set; }
            public Vip vip { get; set; }
        }

        public class Space
        {
            public string s_img { get; set; }
            public string l_img { get; set; }
        }

        public class Data
        {
            public Card card { get; set; }
            public Space space { get; set; }
            public bool following { get; set; }
            public int archive_count { get; set; }
            public int article_count { get; set; }
            public int follower { get; set; }
        }

        public class Root
        {
            public int code { get; set; }
            public string message { get; set; }
            public int ttl { get; set; }
            public Data data { get; set; }
        }

        #endregion

        public UpProcess():base("up")
        {
        }
        public static Request CreateRquerst(int upId)
        {
            // https://api.bilibili.com/x/web-interface/card?mid=3630684&photo=1

            var url = $"https://api.bilibili.com/x/web-interface/card?mid={upId}&photo=1";
            var request = new Request()
            {
                RequestUri = new Uri(url)
            };

            request.Properties[REQUEST_CHECK_PROPERTY_NAME] = "up";

            return request;
        }

        public override void OnHanlder(DataFlowContext context, Root parseObj)
        {
            if (parseObj.data == null)
                return;

            var upId = int.Parse(parseObj.data.card.mid);    // 额，理论上是数字id

            using var db = DBSet.GetCon(DBSet.SqliteDBName.Bilibili);
            var up = db.SingleById<UP>(upId);

            if (up == null)
            {
                // 没有up主信息，则新建，否则只是复制数据
                up = new UP();
                up.Id = upId;
                db.Insert(up);
            }
            var data = parseObj.data.card;
            up.following = data.fans;
            up.follower = data.attention;
            up.friend = data.friend;
            up.video = parseObj.data.archive_count;
            up.name = data.name;
            up.sex = data.sex;
            up.face = data.face;
            up.sign = data.sign;
            up.level = data.level_info.current_level;
            
            db.Update(up);
        }
    }
}
