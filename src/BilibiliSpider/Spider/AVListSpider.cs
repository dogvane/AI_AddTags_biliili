using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BilibiliSpider.Spider.DataProcess;
using BilibiliSpider.Entity.Spider;
using DotnetSpider;
using DotnetSpider.Downloader;
using DotnetSpider.Http;
using DotnetSpider.Scheduler;
using DotnetSpider.Scheduler.Component;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace BilibiliSpider.Spider
{
    /// <summary>
    /// 采集视频列表用
    /// </summary>
    sealed class AVListSpider:DotnetSpider.Spider
    {
        public static Task RunAsync()
        {
            var builder = Builder.CreateDefaultBuilder<AVListSpider>(options =>
            {
                options.Speed = 0.5;
            });
            builder.UseDownloader<HttpClientDownloader>();
            builder.UseSerilog();
            builder.IgnoreServerCertificateError();
            builder.UseQueueDistinctBfsScheduler<HashSetDuplicateRemover>();
            return builder.Build().RunAsync();
        }

        public AVListSpider(IOptions<SpiderOptions> options, DependenceServices services, ILogger<DotnetSpider.Spider> logger) : base(options, services, logger)
        {
            AddDataFlow(new BilibiliListProcess());
            AddDataFlow(new UpProcess());
            AddDataFlow(new DescProcess());
            AddDataFlow(new TagProcess());
            AddDataFlow(new ImageProcess());
        }

        protected override Task InitializeAsync(CancellationToken stoppingToken = default)
        {
            List<Request> requests = new List<Request>();

            foreach (var b in new []{  20, 154, 198, 199, 200, 156 } )
            {
                requests.Add(BilibiliListProcess.CreateListRequest(b, 1));
            }

            return AddRequestsAsync(requests);
        }

    }

}
