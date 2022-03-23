using System;
using Serilog;

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
            Log.Information("ApplicationHandler::Start()");

            _instanceHandler.OnAction();
        }

        public void Stop()
        {
            Log.Information("ApplicationHandler::Stop()");
        }
    }
}