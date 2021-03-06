﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using OpenCvSharp;
using ServiceStack.DataAnnotations;

namespace BilibiliSpider.Entity.Database
{
    /// <summary>
    /// 图片的标签信息
    /// 主要是自己打的标签数据
    /// </summary>
    [Serializable]
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
        /// 当前的状态：
        /// downfile_finish 表示图片下载完成
        /// opencv_fail 表示通过opencv做人脸识别失败
        /// invalid_data 表示opencv分析出来了，但是识别出来的数据是错误的需要被忽略
        /// later 有数据，但是不考虑做分类，稍后再说。
        /// completed 数据处理完成
        /// </summary>
        public string Status{ get; set; }

        /// <summary>
        /// 标签区域
        /// </summary>
        [Ignore]
        public List<TagPart> OpenCvParts { get; set; } = new List<TagPart>();

        [Alias("OpenCvParts")]
        public string PartsStr{
            get
            {
                return JsonConvert.SerializeObject(OpenCvParts);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    OpenCvParts = JsonConvert.DeserializeObject<List<TagPart>>(value);
                }
            }
        }
    }

    /// <summary>
    /// 一个标签的区域
    /// </summary>
    [Serializable]
    public class TagPart
    {
        /// <summary>
        /// 脸部的位置
        /// 不用属性，主要是属性没法反序列化
        /// </summary>
        public Rect Face;

        /// <summary>
        /// 身体的部分
        /// 不用属性，主要是属性没法反序列化
        /// </summary>
        public Rect Body;

        /// <summary>
        /// 标签内容
        /// </summary>
        public string TagNames { get; set; }

        /// <summary>
        /// 状态
        /// 0 未设置
        /// 1 已设置
        /// 2 无效状态
        /// </summary>
        public int State { get; set; }
    }
}
