namespace BilibiliSpider.Entity.Spider
{
    /// <summary>
    ///     UP主的json数据格式
    /// </summary>
    public class UPInfo
    {
        public class Official
        {
            /// <summary>
            /// </summary>
            public int role { get; set; }

            /// <summary>
            /// </summary>
            public string title { get; set; }

            /// <summary>
            /// </summary>
            public string desc { get; set; }
        }

        public class Vip
        {
            /// <summary>
            /// </summary>
            public int type { get; set; }

            /// <summary>
            /// </summary>
            public int status { get; set; }

            /// <summary>
            /// </summary>
            public int theme_type { get; set; }
        }

        public class Theme
        {
        }

        public class Sys_notice
        {
        }

        public class Data
        {
            /// <summary>
            /// </summary>
            public int mid { get; set; }

            /// <summary>
            ///     名字
            /// </summary>
            public string name { get; set; }

            /// <summary>
            ///     性别
            /// 男 女 or 中间吗？
            /// </summary>
            public string sex { get; set; }

            /// <summary>
            ///     头像图片
            ///     如果有的话
            /// </summary>
            public string face { get; set; }

            /// <summary>
            ///     签名
            /// </summary>
            public string sign { get; set; }

            /// <summary>
            /// 排名
            /// </summary>
            public int rank { get; set; }

            /// <summary>
            /// 等级
            /// </summary>
            public int level { get; set; }

            /// <summary>
            /// 入站时间
            /// </summary>
            public int jointime { get; set; }

            /// <summary>
            /// </summary>
            public int moral { get; set; }

            /// <summary>
            /// </summary>
            public int silence { get; set; }

            /// <summary>
            /// </summary>
            public string birthday { get; set; }

            /// <summary>
            /// </summary>
            public int coins { get; set; }

            /// <summary>
            /// </summary>
            public string fans_badge { get; set; }

            /// <summary>
            /// </summary>
            public Official official { get; set; }

            /// <summary>
            /// </summary>
            public Vip vip { get; set; }

            /// <summary>
            /// </summary>
            public string is_followed { get; set; }

            /// <summary>
            ///     导航图片栏图片
            /// </summary>
            public string top_photo { get; set; }

            /// <summary>
            /// </summary>
            public Theme theme { get; set; }

            /// <summary>
            /// </summary>
            public Sys_notice sys_notice { get; set; }
        }

        public class Root
        {
            /// <summary>
            /// </summary>
            public int code { get; set; }

            /// <summary>
            /// </summary>
            public string message { get; set; }

            /// <summary>
            /// </summary>
            public int ttl { get; set; }

            /// <summary>
            /// </summary>
            public Data data { get; set; }
        }
    }

    /// <summary>
    ///     粉丝的数据
    /// </summary>
    public class UPFace
    {
        public class Data
        {
            /// <summary>
            /// </summary>
            public int mid { get; set; }

            /// <summary>
            /// </summary>
            public int following { get; set; }

            /// <summary>
            /// </summary>
            public int whisper { get; set; }

            /// <summary>
            /// </summary>
            public int black { get; set; }

            /// <summary>
            /// </summary>
            public int follower { get; set; }
        }

        public class Root
        {
            /// <summary>
            /// </summary>
            public int code { get; set; }

            /// <summary>
            /// </summary>
            public string message { get; set; }

            /// <summary>
            /// </summary>
            public int ttl { get; set; }

            /// <summary>
            /// </summary>
            public Data data { get; set; }
        }
    }

    public class UPStat
    {
        public class Archive
        {
            /// <summary>
            /// 
            /// </summary>
            public int view { get; set; }
        }

        public class Article
        {
            /// <summary>
            /// 
            /// </summary>
            public int view { get; set; }
        }

        public class Data
        {
            /// <summary>
            /// 
            /// </summary>
            public Archive archive { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Article article { get; set; }
        }

        public class Root
        {
            /// <summary>
            /// 
            /// </summary>
            public int code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string message { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int ttl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Data data { get; set; }
        }
    }
}
