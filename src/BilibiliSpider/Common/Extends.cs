using System;
using System.Collections.Generic;
using System.Text;

namespace BilibiliSpider.Common
{
    public static class Extends
    {
        public static bool CheckQueryType(this IDictionary<string, object> props, string typeName)
        {
            if (!props.ContainsKey("queryType"))
                return false;

            return (string) props["queryType"] == typeName;
        }

        /// <summary>
        /// 转逗号分隔的字符串
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string ToCSV(List<string> items)
        {
            if (items == null || items.Count == 0)
                return string.Empty;

            StringBuilder ret = new StringBuilder();

            foreach (var item in items)
            {
                ret.Append(item).Append(',');
            }

            ret.Remove(ret.Length - 1, 1);

            return ret.ToString();
        }

        /// <summary>
        /// 转逗号分隔的字符串
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string ToCSV(string[] items)
        {
            if (items == null || items.Length == 0)
                return string.Empty;

            StringBuilder ret = new StringBuilder();

            foreach (var item in items)
            {
                ret.Append(item).Append(',');
            }

            ret.Remove(ret.Length - 1, 1);

            return ret.ToString();
        }
    }
}
