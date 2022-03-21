using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archie
{
    class Startup
    {

        IConfiguration Configuration { get; }
    
        IServiceProvider ServiceProvider { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(Configuration)
              //.Enrich.FromLogContext()
              //.WriteTo.File(shared:true,path: configuration["Logging:LogLevel:Default"])
              .CreateLogger();
            Globals.Configuration = configuration;

        }
        public void ConfigureServices(IServiceCollection services)
        {

           
            services.AddLogging(configure => configure.AddSerilog());
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IReportCollector, iSiteReportCollector>();




            // services.AddSingleton<IReportCollector>(ctx=>new iSiteReportCollector(ctx.GetService<ILoggerFactory>(),Configuration));


        }

    }
}
