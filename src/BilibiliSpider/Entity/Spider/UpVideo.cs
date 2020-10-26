using System.Collections.Generic;

namespace BilibiliSpider.Entity.Spider
{
    public class UpVideo
    {
        /// <summary>
        /// 应该是舞曲的分组吧
        /// </summary>
        public class TListItem
        {
            /// <summary>
            /// </summary>
            public int tid { get; set; }

            /// <summary>
            /// </summary>
            public int count { get; set; }

            /// <summary>
            ///     
            /// </summary>
            public string name { get; set; }
        }

        /// <summary>
        /// 视频的列表数据
        /// </summary>
        public class VlistItem
        {
            /// <summary>
            /// 评论的数量
            /// </summary>
            public int comment { get; set; }

            /// <summary>
            /// tag分组id吧
            /// </summary>
            public int typeid { get; set; }

            /// <summary>
            /// 播放量
            /// </summary>
            public int play { get; set; }

            /// <summary>
            /// 骗人的封面照
            /// </summary>
            public string pic { get; set; }

            /// <summary>
            /// </summary>
            public string subtitle { get; set; }

            /// <summary>
            ///     描述
            /// </summary>
            public string description { get; set; }

            /// <summary>
            /// 版权信息
            /// </summary>
            public string copyright { get; set; }

            /// <summary>
            ///     标题
            /// </summary>
            public string title { get; set; }

            /// <summary>
            /// </summary>
            public int review { get; set; }

            /// <summary>
            ///     萧佩玖
            /// </summary>
            public string author { get; set; }

            /// <summary>
            /// 视频id
            /// </summary>
            public int mid { get; set; }

            /// <summary>
            /// 是否是联合投稿
            /// </summary>
            public int is_union_video { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            public int created { get; set; }

            /// <summary>
            /// 视频长度，HH:mm:ss
            /// </summary>
            public string length { get; set; }

            /// <summary>
            /// </summary>
            public int video_review { get; set; }

            /// <summary>
            /// </summary>
            public int is_pay { get; set; }

            /// <summary>
            /// 收藏
            /// </summary>
            public int favorites { get; set; }

            /// <summary>
            /// up主id
            /// </summary>
            public int aid { get; set; }

            /// <summary>
            /// </summary>
            public string hide_click { get; set; }
        }

        public class Data
        {
            /// <summary>
            /// </summary>
            public Dictionary<int, TListItem> tlist { get; set; }

            /// <summary>
            /// </summary>
            public List<VlistItem> vlist { get; set; }

            /// <summary>
            /// </summary>
            public int count { get; set; }

            /// <summary>
            /// </summary>
            public int pages { get; set; }
        }

        public class Root
        {
            /// <summary>
            /// </summary>
            public string status { get; set; }

            /// <summary>
            /// </summary>
            public Data data { get; set; }
        }
    }
}
