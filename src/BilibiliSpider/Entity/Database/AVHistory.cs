using System;
using System.Collections.Generic;
using System.Text;
using BilibiliSpider.Entity.Spider;

namespace BilibiliSpider.Entity.Database
{
    /// <summary>
    /// AV 的采集历史
    /// </summary>
    public class AVHistory
    {
        public int Id { get; set; }

        public List<ArchivesItem> History { get; set; }
    }
}
