using System.Collections.Generic;

namespace BilibiliSpider.Entity.Spider
{
    public class Rights
    {
        /// <summary>
        /// </summary>
        public int bp { get; set; }

        /// <summary>
        /// </summary>
        public int elec { get; set; }

        /// <summary>
        /// </summary>
        public int download { get; set; }

        /// <summary>
        /// </summary>
        public int movie { get; set; }

        /// <summary>
        /// </summary>
        public int pay { get; set; }

        /// <summary>
        /// </summary>
        public int hd5 { get; set; }

        /// <summary>
        /// </summary>
        public int no_reprint { get; set; }

        /// <summary>
        /// </summary>
        public int autoplay { get; set; }

        /// <summary>
        /// </summary>
        public int ugc_pay { get; set; }

        /// <summary>
        /// </summary>
        public int is_cooperation { get; set; }

        /// <summary>
        /// </summary>
        public int ugc_pay_preview { get; set; }
    }

    /// <summary>
    /// Up主
    /// </summary>
    public class Owner
    {
        /// <summary>
        /// </summary>
        public int mid { get; set; }

        /// <summary>
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// </summary>
        public string face { get; set; }
    }

    /// <summary>
    /// 视频状态
    /// </summary>
    public class Stat
    {
        /// <summary>
        /// 视频id
        /// </summary>
        public int aid { get; set; }

        /// <summary>
        /// 观影人物
        /// </summary>
        public int view { get; set; }

        /// <summary>
        /// 弹幕数量
        /// </summary>
        public int danmaku { get; set; }

        /// <summary>
        /// 评论
        /// </summary>
        public int reply { get; set; }

        /// <summary>
        /// 收藏
        /// </summary>
        public int favorite { get; set; }

        /// <summary>
        /// 硬币
        /// </summary>
        public int coin { get; set; }

        /// <summary>
        /// 分享
        /// </summary>
        public int share { get; set; }

        /// <summary>
        /// 当前排名
        /// </summary>
        public int now_rank { get; set; }

        /// <summary>
        /// 历史最高排名
        /// </summary>
        public int his_rank { get; set; }

        /// <summary>
        /// 喜欢
        /// </summary>
        public int like { get; set; }

        /// <summary>
        /// 不喜欢（不过，没找到点击的地方）
        /// </summary>
        public int dislike { get; set; }
    }

    public class Dimension
    {
        /// <summary>
        /// </summary>
        public int width { get; set; }

        /// <summary>
        /// </summary>
        public int height { get; set; }

        /// <summary>
        /// </summary>
        public int rotate { get; set; }
    }

    public class ArchivesItem
    {
        /// <summary>
        /// </summary>
        public int aid { get; set; }

        /// <summary>
        /// av下有多少个视频
        /// </summary>
        public int videos { get; set; }

        /// <summary>
        /// tag 的id 20是舞曲
        /// </summary>
        public int tid { get; set; }

        /// <summary>
        ///     tag 的名字
        /// </summary>
        public string tname { get; set; }

        /// <summary>
        /// 版权信息 1=原创   2=转载
        /// </summary>
        public int copyright { get; set; }

        /// <summary>
        /// 骗流量的封面照
        /// </summary>
        public string pic { get; set; }

        /// <summary>
        ///     标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 应该是发布时间
        /// </summary>
        public int pubdate { get; set; }

        /// <summary>
        /// 创建时间（应该是首次上传的时间吧）
        /// </summary>
        public int ctime { get; set; }

        /// <summary>
        ///     描述
        /// </summary>
        public string desc { get; set; }

        /// <summary>
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// </summary>
        public int attribute { get; set; }

        /// <summary>
        /// 时长，单位应该是秒
        /// </summary>
        public int duration { get; set; }

        /// <summary>
        /// 看上去应该是版权信息
        /// </summary>
        public Rights rights { get; set; }

        /// <summary>
        /// Up主信息
        /// </summary>
        public Owner owner { get; set; }

        /// <summary>
        /// 视频状态
        /// </summary>
        public Stat stat { get; set; }

        /// <summary>
        ///     #萌妹子##宅舞##可爱#
        /// </summary>
        public string dynamic { get; set; }

        /// <summary>
        /// 不明觉厉
        /// </summary>
        public int cid { get; set; }

        /// <summary>
        /// </summary>
        public Dimension dimension { get; set; }
    }

    public class Page
    {
        /// <summary>
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// </summary>
        public int num { get; set; }

        /// <summary>
        /// </summary>
        public int size { get; set; }
    }

    public class AVDataReturn
    {
        /// <summary>
        /// </summary>
        public List<ArchivesItem> archives { get; set; }

        /// <summary>
        /// </summary>
        public Page page { get; set; }
    }

    public class NewListReturn
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
        public AVDataReturn data { get; set; }
    }
}
