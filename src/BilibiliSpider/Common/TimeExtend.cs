using System;

namespace BilibiliSpider.Common
{
    /// <summary>
    /// 时间的一些扩展方法
    /// </summary>
    public static class TimeExtend
    {
        static DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        public static DateTime UnixToDateTime(this int time)
        {
            return startTime.AddSeconds(time);
        }
    }
}
