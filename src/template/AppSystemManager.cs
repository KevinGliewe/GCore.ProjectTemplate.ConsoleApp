using Serilog;
using Anotar.Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace GCore.ProjectTemplate.ConsoleApp {
    public class AppSystemManager {
        public IConfiguration Config { get; private set; }
        public IServiceProvider Services { get; private set; }
        public ILogger Logger { get; private set; }
        public Config.ApplicationOptions Options { get; private set; } = new Config.ApplicationOptions();

        public AppSystemManager(Action<IConfigurationBuilder> configBuild, Action<LoggerConfiguration, IConfiguration> loggerBuild, Action<IServiceCollection, IConfiguration> serviceBuild) {
            // Initialize Configuration
            var configurationBuilder = new ConfigurationBuilder();
            ConfigBuild(configurationBuilder);
            configBuild?.Invoke(configurationBuilder);
            Config = configurationBuilder.Build();

            var prevConColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Config:");
            foreach (var entry in Config.AsEnumerable())
                Console.WriteLine($"  {entry.Key} = {entry.Value}");
            Console.WriteLine();
            Console.ForegroundColor = prevConColor;

            // Initialize Logging
            var loggerConfiguration = new LoggerConfiguration();
            LoggerBuild(loggerConfiguration);
            loggerBuild?.Invoke(loggerConfiguration, Config);
            Logger = Log.Logger = loggerConfiguration.CreateLogger();

            // Initialize Services
            var serviceCollection = new ServiceCollection();
            ServiceBuild(serviceCollection);
            serviceBuild?.Invoke(serviceCollection, Config);
            Services = serviceCollection.BuildServiceProvider();

            Options = Services.GetService<Config.ApplicationOptions>();
        }

        private void ConfigBuild(IConfigurationBuilder builder) {

        }

        private void LoggerBuild(LoggerConfiguration builder) {
            builder
                .ReadFrom.Configuration(Config);
        }

        private void ServiceBuild(IServiceCollection builder) {
            builder
                .AddSingleton(Logger)
                .AddSingleton(Config);
        }

        public static DateTime GetBuildDate(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<BuildDateAttribute>();
            return attribute != null ? attribute.DateTime : default(DateTime);
        }
    }
}