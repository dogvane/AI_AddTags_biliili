using System;
using System.Collections.Generic;
using System.Text;
using BaiduFaceAI.Entity;
using ServiceStack.DataAnnotations;

namespace BilibiliSpider.Entity.Database
{
    /// <summary>
    /// 图片检查
    /// </summary>
    public class ImageDetect
    {
        /// <summary>
        /// ID自增
        /// </summary>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// 关联影片id
        /// </summary>
        public int AVId { get; set; }

        /// <summary>
        /// UP主id
        /// </summary>
        public int UpId { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 图片本地路径
        /// </summary>
        public string LocalFile { get; set; }

        /// <summary>
        /// 图片有多少张脸
        /// </summary>
        public int face_num { get; set; }

        /// <summary>
        /// 人脸置信度，范围【0~1】，代表这是一张人脸的概率，0最小、1最大。
        /// 多张照片，取最大的数值
        /// </summary>
        public double max_face_probability { get; set; }

        /// <summary>
        /// 根据百度官方文档里，适用于验证的质量的一个统计值
        /// 内部计算用，从0~1， 1表示最好
        /// 只要有一项不满足验证要求的，都为0
        /// </summary>
        public double max_quality { get; set; }

        /// <summary>
        /// 检测的返回
        /// </summary>
        public DetectResult Detect { get; set; }

        /// <summary>
        /// 是否已经添加到DB数据库里过了
        /// </summary>
        public bool AddToFaceDB { get; set; }
    }
}
