using ISITELib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Archie
{
    class Program
    {
        static async Task Main(string[] args)
        {







            //var hostBuilder = CreateHostBuilder(args);
            //var host = hostBuilder.Build();
            //var serviceProvider = host.Services;

            //var logFactory = serviceProvider.GetService<ILoggerFactory>();
            //Globals.LoggerFactory = logFactory;

            //var logger = logFactory.CreateLogger<Program>();


           
            var host = AppStartup();
            var serviceProvider = host.Services;

            var collector = serviceProvider.GetService<IReportCollector>();
            var configuration = Globals.Configuration;
            var startDt = configuration["Collector:From:iSite:Query:StartDt"];
            var endDt = configuration["Collector:From:iSite:Query:EndDt"];
            var accessionList = configuration["Collector:From:iSite:Query:AccessionList"];

            String queryWithDateVariables = configuration["Collector:From:iSite:Query:QueryString"];
            int maxQueryResults = configuration.GetValue<int>("Collector:From:iSite:Query:MaxQueryResults");

            int inervalQuery = configuration.GetValue<int>("Collector:From:iSite:Query:QueryInterval");

            if (!string.IsNullOrEmpty(accessionList))
            {
                var list = accessionList.Split(';');
                Queue<string> _accQueue = new Queue<string>(list);

                collector.CollectData(_accQueue);
            }
            else
                collector.CollectData(DateTime.Parse(startDt), DateTime.Parse(endDt));

            //collector.CollectData(new DateTime(2011,11,3),new DateTime(2011,11,4));

            // collector.Initialize();
            await host.RunAsync();

        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args)
         .ConfigureAppConfiguration((hostingContext, config) =>
         {
             var env = hostingContext.HostingEnvironment;
             config.AddJsonFile("archie.configuration.json", optional: true, reloadOnChange: true)
             .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
         }).UseStartup<Startup>();


        static void BuildConfig(IConfigurationBuilder builder)
        {
            // Check the current directory that the application is running on 
            // Then once the file 'appsetting.json' is found, we are adding it.
            // We add env variables, which can override the configs in appsettings.json
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("archie.configuration.json", optional: true, reloadOnChange: true)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }



        static IHost AppStartup()
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);
            var conf = builder.Build();
            Globals.Configuration = conf;
            // Specifying the configuration for serilog
            Log.Logger = new LoggerConfiguration() // initiate the logger configuration
                            .ReadFrom.Configuration(conf) // connect serilog to our configuration folder
                            .Enrich.FromLogContext() //Adds more information to our logs from built in Serilog 
                            .WriteTo.Console() // decide where the logs are going to be shown
                            .CreateLogger(); //initialise the logger

            Log.Logger.Information("Application Starting");

            String queryWithDateVariables = conf["Collector:From:iSite:Query:QueryString"];
            int maxQueryResults = conf.GetValue<int>("Collector:From:iSite:Query:MaxResults");

            int inervalQuery = conf.GetValue<int>("Collector:From:iSite:Query:QueryInterval");
            iSiteNonVisualClass _myiSite = null;
            try
            {
                _myiSite = new iSiteNonVisualClass();
            }
            catch (Exception)
            {
                _myiSite = null;
                Log.Logger.Information("Could not create isite");
            }

            var iSyntaxServer = conf["Collector:From:iSite:iSyntaxServerIP"];
          
            _myiSite.ImageSuiteURL = $"http://{iSyntaxServer}/iSiteWeb/WorkList/PrimaryWorkList.ashx";
            _myiSite.ImageSuiteDSN = "iSite"; // "iSite";
            _myiSite.iSyntaxServerIP = iSyntaxServer;
            _myiSite.iSyntaxServerPort = "6464";
            _myiSite.Options = "StentorBackEnd";




            Thread.Sleep(300);
            bool ret = _myiSite.Initialize();

           
            int err = 0;
            err = _myiSite.GetLastErrorCode();
            Console.WriteLine("Hello World!" + err);

            var host = Host.CreateDefaultBuilder() // Initialising the Host 
                        .ConfigureServices((context, services) => { // Adding the DI container for configuration
                            services.AddLogging(configure => configure.AddSerilog());
                            services.AddSingleton<IConfiguration>(conf);
                            services.AddSingleton<iSiteNonVisualClass>(_myiSite);
                            services.AddSingleton<IReportCollector,iSiteReportCollector>();
                        })
                        .Build(); // Build the Host

            return host;
        }
    }


    public static class Globals
    {
        public static IConfiguration Configuration { get; set; }
        public static ILoggerFactory LoggerFactory {get;set;}
    }

    public static class HostBuilderExtensions
    {
        private const string ConfigureServicesMethodName = "ConfigureServices";

        /// <summary>
        /// Specify the startup type to be used by the host.
        /// </summary>
        /// <typeparam name="TStartup">The type containing an optional constructor with
        /// an <see cref="IConfiguration"/> parameter. The implementation should contain a public
        /// method named ConfigureServices with <see cref="IServiceCollection"/> parameter.</typeparam>
        /// <param name="hostBuilder">The <see cref="IHostBuilder"/> to initialize with TStartup.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public static IHostBuilder UseStartup<TStartup>(
            this IHostBuilder hostBuilder) where TStartup : class
        {
            // Invoke the ConfigureServices method on IHostBuilder...
            hostBuilder.ConfigureServices((ctx, serviceCollection) =>
            {
                // Find a method that has this signature: ConfigureServices(IServiceCollection)
                var cfgServicesMethod = typeof(TStartup).GetMethod(
                    ConfigureServicesMethodName, new Type[] { typeof(IServiceCollection) });

                // Check if TStartup has a ctor that takes a IConfiguration parameter
                var hasConfigCtor = typeof(TStartup).GetConstructor(
                    new Type[] { typeof(IConfiguration) }) != null;

                // create a TStartup instance based on ctor
                var startUpObj = hasConfigCtor ?
                    (TStartup)Activator.CreateInstance(typeof(TStartup), ctx.Configuration) :
                    (TStartup)Activator.CreateInstance(typeof(TStartup), null);

                // finally, call the ConfigureServices implemented by the TStartup object
                cfgServicesMethod?.Invoke(startUpObj, new object[] { serviceCollection });
            });

            // chain the response
            return hostBuilder;
        }
    }




  
}
