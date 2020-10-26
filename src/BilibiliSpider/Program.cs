using System;
using System.IO;
using System.Net;
using BilibiliSpider.Spider;
using BilibiliSpider.DB;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace BilibiliSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console().WriteTo.RollingFile("logs/spiders.log")
                .CreateLogger();

            Console.WriteLine("Hello World!");
            DBSet.Init();

            var task = AVListSpider.RunAsync();
            task.Wait();

            Console.WriteLine("finish.");
        }
    }
}
