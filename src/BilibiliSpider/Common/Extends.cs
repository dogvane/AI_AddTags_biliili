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
    }
}
