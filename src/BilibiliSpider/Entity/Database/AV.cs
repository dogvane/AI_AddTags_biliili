using System;
using System.Collections.Generic;
using System.Text;
using BilibiliSpider.Entity.Spider;
using ServiceStack.DataAnnotations;

namespace BilibiliSpider.Entity.Database
{
    /// <summary>
    /// 字面意思
    /// </summary>
    public class AV
    {

        /// <summary>
        /// av id 应该都是数字的
        /// </summary>
        [PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// 新的 bvid
        /// </summary>
        public string bvId{ get; set; }

        /// <summary>
        /// up主的id，
        /// </summary>
        public int UpId { get; set; }

        /// <summary>
        ///     标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 视频的描述信息
        /// </summary>
        public string desc { get; set; }

        /// <summary>
        /// av下有多少个视频
        /// </summary>
        public int videos { get; set; }

        /// <summary>
        /// 创建时间（应该是首次上传的时间吧）
        /// </summary>
        public int ctime { get; set; }

        /// <summary>
        /// 创建时间（应该是首次上传的时间吧）
        /// </summary>
        public string ctime2 { get; set; }

        /// <summary>
        /// 骗流量的封面照
        /// </summary>
        public string pic { get; set; }

        /// <summary>
        /// 版权类型
        /// </summary>
        public int copyright { get; set; }

        /// <summary>
        /// 视频状态
        /// </summary>
        public Stat stat { get; set; }

        /// <summary>
        /// 将观影人数列出来
        /// </summary>
        public int view { get; set; }

        /// <summary>
        /// 板块id
        /// 宅舞 20
        /// 三次元（舞蹈综合） 154,
        /// 街舞 198,
        /// 明星舞蹈 199
        /// 中国舞蹈 200,
        /// 舞蹈教程 156
        /// </summary>
        public int rid { get; set; }

        /// <summary>
        /// 抓取弹幕需要这个cid
        /// 
        /// </summary>
        public int cid{ get; set; }


        /// <summary>
        /// 标签的Id列表，主要是哪里做数据库比对的
        /// </summary>
        public string tagIds{ get; set; }

        /// <summary>
        /// 标签的名字，用来展示用的，和tagIds是对应关系
        /// </summary>
        public string tagNames{ get; set; }
    }
}
