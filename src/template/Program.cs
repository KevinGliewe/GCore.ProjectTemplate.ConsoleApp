using System;
using System.Collections.Generic;
using Anotar.Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace GCore.ProjectTemplate.ConsoleApp {
    class Program {
        public static readonly string ENV_PREFIX = "GCore.ProjectTemplate.ConsoleApp";
        public static AppSystemManager SystemManager { get; private set; }
        static void Main(string[] args) {

            Console.WriteLine("FileVersion          : " + AssemblyVersionConstants.FileVersion);
            Console.WriteLine("InformationalVersion : " + AssemblyVersionConstants.InformationalVersion);
            Console.WriteLine("Build Timestamp      : " + AppSystemManager.GetBuildDate(typeof(Program).Assembly));
            Console.WriteLine();

            try {
                SystemManager = new AppSystemManager(ConfigBuild, LoggerBuild, ServiceBuild);

                var host = SystemManager.Services.GetService<Handler.ApplicationHandler>();
                host.Start();

                //Console.ReadLine();

                host.Stop();
            } catch (Exception ex) {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("System Crash: " + ex);
                Console.BackgroundColor = ConsoleColor.Black;
                
                if(SystemManager.Options.ReadLineAfterException)
                    Console.ReadLine();
            }

        }

        private static void ConfigBuild(IConfigurationBuilder builder) {
            var prefix = ENV_PREFIX.ToUpper().Replace('.', '_') + "_";

            var env = Environment.GetEnvironmentVariable($"{prefix}ENV") ??
#if (DEBUG)
            "Development";
#else
            "Production";
#endif
            Console.WriteLine("Environment: " + env);

            builder.AddInMemoryCollection(new Dictionary<string, string>() {
                    { "Application:Option", "InMemory" },
                    { "InstancedHandler", "GCore.ProjectTemplate.ConsoleApp.Handler.InstancedHandler" },

                    { "Serilog:WriteTo:0:Name", "Console" },
                })
                .AddJsonFile("appsettings.json", optional: true)
                .AddXmlFile("appsettings.xml", optional: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddXmlFile($"appsettings.{env}.xml", optional: true)
                .AddEnvironmentVariables(prefix)
                .AddEnvironmentVariables($"{prefix}{env}_")
                .AddCommandLine(Environment.GetCommandLineArgs());
        }

        private static void LoggerBuild(LoggerConfiguration builder, IConfiguration config) {
        }

        private static void ServiceBuild(IServiceCollection builder, IConfiguration config) {
            builder
                .AddSingleton(config.GetSection("Application").Get<Config.ApplicationOptions>())
                .AddSingleton<Handler.ApplicationHandler>();

            // IInstancedHandler --------------------------------------------------------------------
            var instanceHandlerClassName = config["InstancedHandler"];
            var instanceHandlerType = Type.GetType(instanceHandlerClassName);

            if (instanceHandlerType is null) {
                LogTo.Error($"Can't find IInstancedHandler '{instanceHandlerClassName}'");
                return;
            } else {
                LogTo.Information($"Using IInstancedHandler '{instanceHandlerType}'");
            }

            builder.AddSingleton(typeof(Handler.IInstancedHandler), instanceHandlerType);
        }
    }
}
