using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.DataAnnotations;

namespace BilibiliSpider.Entity.Database
{
    /// <summary>
    /// Up主
    /// </summary>
    public class UP
    {
        /// <summary>
        /// Up 主的id ，应该都是数字的
        /// 一般来说，会对应到mid
        /// </summary>
        [PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// 粉丝数量
        /// </summary>
        public int following { get; set; }

        /// <summary>
        /// 关注的人数
        /// </summary>
        public int follower { get; set; }

        /// <summary>
        /// 好友数量，应该是指互相关注了的人的数量
        /// </summary>
        public int friend{ get; set; }

        /// <summary>
        /// 视频播放数量
        /// 现在没返回了
        /// </summary>
        public int views { get; set; }

        /// <summary>
        /// 视频数量
        /// </summary>
        public int video { get; set; }

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
        /// 这个现在也没了
        /// </summary>
        public int jointime { get; set; }

        /// <summary>
        /// 特殊的称号信息
        /// </summary>
        public Nameplate nameplate{ get; set; }

    }

    /// <summary>
    /// 这里好像是一些特殊的称号信息
    /// </summary>
    public class Nameplate
    {
        public int nid { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string image_small { get; set; }
        public string level { get; set; }
        public string condition { get; set; }
    }
}
