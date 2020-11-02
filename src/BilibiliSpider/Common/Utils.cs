using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BilibiliSpider.Common
{
    /// <summary>
    /// 辅助工具操作
    /// </summary>
    public static class Utils
    {
        static string defaultDataFolder;

        /// <summary>
        /// 默认的数据目录
        /// 一些其它相关数据目录都基于该目录处理
        /// 
        /// </summary>
        public static string DefaultDataFolder
        {
            get
            {
                if (defaultDataFolder == null)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        var info = new DirectoryInfo(AppContext.BaseDirectory);

                        while (info.Parent != null)
                        {
                            info = info.Parent;
                        }

                        defaultDataFolder = Path.Combine(info.Name, @"Data/");
                    }
                    else
                    {
                        // todo 通过配置文件做初始化
                    }
                }

                return defaultDataFolder;
            }
        }
    }
}
