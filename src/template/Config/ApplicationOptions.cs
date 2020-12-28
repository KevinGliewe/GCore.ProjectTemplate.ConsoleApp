using System;


namespace GCore.ProjectTemplate.ConsoleApp.Config {
    public class ApplicationOptions {
        public string Option { get; set; } = "default";
        public bool ReadLineAfterException { get; set; } = true;
    }
}