using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using GCore.AppSystem;
using GCore.AppSystem.Config;
using GCore.AppSystem.Extensions;
using GCore.AppSystem.Handler;
using GCore.Logging.Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace GCore.ProjectTemplate.ConsoleApp
{
    class Program
    {
        public static readonly string ENV_PREFIX = "GCore.ProjectTemplate.ConsoleApp";
        public static IAppSystemManager SystemManager { get; private set; }

        static void Main(string[] args)
        {

            Console.WriteLine("FileVersion          : " + AssemblyVersionConstants.FileVersion);
            Console.WriteLine("InformationalVersion : " + AssemblyVersionConstants.InformationalVersion);
            Console.WriteLine("Build Timestamp      : " + GetBuildDate(typeof(Program).Assembly));
            Console.WriteLine();

            try
            {
                var builder = new AppSystemManagerBuilder();
                builder.AddScannableAssemblies(typeof(Program).Assembly);

                SystemManager = builder.Build();

                var host = SystemManager.Services.GetService<Handler.ApplicationHandler>();
                host.Start();

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();

                host.Stop();
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("System Crash: " + ex);
                Console.BackgroundColor = ConsoleColor.Black;

                if (SystemManager.Services.GetService<Config.ApplicationOptions>().ReadLineAfterException)
                    Console.ReadLine();
            }

            Log.CloseAndFlush();
        }

        public class AppManagerExtension : IAppSystemExtension
        {
            public void Dispose()
            {
            }


            public void ConfigBuild(IConfigurationBuilder builder, IEnumerable<Assembly> assemblies)
            {
                var prefix = ENV_PREFIX.ToUpper().Replace('.', '_') + "_";

                var env = Environment.GetEnvironmentVariable($"{prefix}ENV") ??
#if (DEBUG)
                "Development";
#else
                "Production";
#endif
                Log.Information("Environment: " + env);

                builder.AddInMemoryCollection(new Dictionary<string, string>()
                    {
                        {"Application:Option", "InMemory"},
                        {"Handlers:InstancedHandler", "GCore.ProjectTemplate.ConsoleApp.Handler.InstancedHandler"},
                        {"Serilog:WriteTo:0:Name", "Console"},
                    })
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddXmlFile("appsettings.xml", optional: true)
                    .AddJsonFile($"appsettings.{env}.json", optional: true)
                    .AddXmlFile($"appsettings.{env}.xml", optional: true)
                    .AddEnvironmentVariables(prefix)
                    .AddEnvironmentVariables($"{prefix}{env}_")
                    .AddCommandLine(Environment.GetCommandLineArgs());
            }

            public void ServiceBuild(IConfiguration config, ContainerBuilder builder, IEnumerable<Assembly> assemblies)
            {
                // Initialize Logging
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(config)
                    .CreateLogger();

                builder
                    .AddSingleton<ILogger>(Log.Logger)
                    .AddSingleton<Handler.ApplicationHandler>();

                ConfigOptionAttribute.BuildServiceOptions(builder, config, assemblies);
                HandlerAttribute.BuildServiceHandlers(builder, config, assemblies);
            }
        }

        public static DateTime GetBuildDate(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<BuildDateAttribute>();
            return attribute != null ? attribute.DateTime : default(DateTime);
        }
    }
}
