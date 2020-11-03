using System;
using System.Collections.Generic;
using System.Text;
using OpenCvSharp;
using ServiceStack.DataAnnotations;

namespace BilibiliSpider.Entity.Database
{
    /// <summary>
    /// 图片的标签信息
    /// </summary>
    public class ImageTag
    {
        /// <summary>
        /// 自增id做主键
        /// </summary>
        [PrimaryKey]
        [AutoIncrement]
        public int Id{ get; set; }

        /// <summary>
        /// 图片在网络上的地址
        /// </summary>
        public string ImageUrl{ get; set; }

        /// <summary>
        /// 图片来源那个AVid
        /// 分析数据用
        /// </summary>
        public int AvId{ get; set; }

        /// <summary>
        /// 图片来源于那个Up主Id
        /// 分析数据用
        /// </summary>
        public int UpId{ get; set; }

        /// <summary>
        /// 本地文件目录名称（相对路径）
        /// 在使用时，需要组合当前执行路径来做判断文件的存在性
        /// </summary>
        public string LocalFileName{ get; set; }

        /// <summary>
        /// 标签的名称列表
        /// 仅名字，逗号分隔
        /// </summary>
        public string TagsName { get; set; }

        /// <summary>
        /// 当前抓过你太
        /// </summary>
        public string Status{ get; set; }

        /// <summary>
        /// 标签区域
        /// </summary>
        public List<TagPart> Parts { get; set; } = new List<TagPart>();
    }

    /// <summary>
    /// 一个标签的区域
    /// </summary>
    public class TagPart
    {
        /// <summary>
        /// 脸部的位置
        /// </summary>
        public Rect Face { get; set; }

        /// <summary>
        /// 身体的部分
        /// </summary>
        public Rect Body { get; set; }

        /// <summary>
        /// 标签内容
        /// </summary>
        public string TagNams { get; set; }
    }
}
