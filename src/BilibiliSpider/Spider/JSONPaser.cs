using System.Threading.Tasks;
using DotnetSpider.DataFlow;
using DotnetSpider.DataFlow.Parser;
using Newtonsoft.Json;

namespace BilibiliSpider.Spider
{
    /// <summary>
    ///     假设返回的数据时json字符串，那么用可以通过改处理类，先将结果转为对应的对象来处理
    ///     并以类型 typof(T).Name 为key，放入context的data结构里，供后面的数据处理使用
    ///     当然，需要做json对象转换的类，也需要再请求参数里带入 jsonParse
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JsonPaser<T> : DataParser
    {
        public const string REQUEST_CHECK_PROPERTY_NAME = "JsonParseType";

        private readonly string _parserName = typeof(T).Name;

        public JsonPaser()
        {
        }

        public JsonPaser(string parserName)
        {
            this._parserName = parserName;
        }

        protected override async Task ParseAsync(DataFlowContext context)
        {
            var props = context.Request.Properties;

            if (!props.ContainsKey(REQUEST_CHECK_PROPERTY_NAME))
                return;

            var type = props[REQUEST_CHECK_PROPERTY_NAME] as string;

            if (type != _parserName)
                return;

            var jsonStr = context.Response.ReadAsString();

            var obj = JsonConvert.DeserializeObject<T>(jsonStr);

            context.AddData(typeof(T).Name, obj);

            OnHanlder(context, obj);
        }

        public abstract void OnHanlder(DataFlowContext context, T parseObj);
    }
}
