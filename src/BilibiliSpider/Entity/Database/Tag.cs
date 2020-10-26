using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.DataAnnotations;

namespace BilibiliSpider.Entity.Database
{
    public class Tag
    {
        /// <summary>
        /// 标签的Id
        /// </summary>
        [PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// 标签的名词
        /// </summary>
        public string name { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public int ctime { get; set; }

        /// <summary>
        /// 标签的头像，虽然不知道有什么用
        /// </summary>
        public string cover { get; set; }

        /// <summary>
        /// 标签的头像，虽然不知道有什么用
        /// </summary>
        public string head_cover { get; set; }

        /// <summary>
        /// 订阅数量
        /// </summary>
        public int subscribed_count { get; set; }

        /// <summary>
        /// tag下归档数量
        /// </summary>
        public string archive_count { get; set; }

        /// <summary>
        /// 特色（不明白的意思）
        /// </summary>
        public int featured_count { get; set; }

        /// <summary>
        /// 不清楚，但感觉是推荐或者排序规则用
        /// </summary>
        public int alpha { get; set; }

        /// <summary>
        /// ui展示用颜色，有颜色的tag会显得高级一些
        /// </summary>
        public string color { get; set; }

        /// <summary>
        /// 先保留
        /// </summary>
        public string tag_type { get; set; }

        /// <summary>
        /// 介绍
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string short_content { get; set; }

        /// <summary>
        /// 使用的数量
        /// </summary>
        public int use{ get; set; }
    }
}
