using System;
using Anotar.Serilog;

namespace GCore.ProjectTemplate.ConsoleApp.Handler {
    public class ApplicationHandler {
        Config.ApplicationOptions _options;
        IInstancedHandler _instanceHandler;

        public ApplicationHandler(Config.ApplicationOptions options, IInstancedHandler instanceHandler) {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _instanceHandler = instanceHandler ?? throw new ArgumentNullException(nameof(instanceHandler));
        }

        public void Start()
        {
            LogTo.Information("ApplicationHandler::Start()");

            _instanceHandler.OnAction();
        }

        public void Stop()
        {
            LogTo.Information("ApplicationHandler::Stop()");
        }
    }
}