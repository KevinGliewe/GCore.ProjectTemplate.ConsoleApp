using System;
using GCore.AppSystem.Config;

namespace GCore.ProjectTemplate.ConsoleApp.Config {

    [ConfigOption("Application")]
    public class ApplicationOptions {
        public string Option { get; set; } = "default";
        public bool ReadLineAfterException { get; set; } = true;
    }
}