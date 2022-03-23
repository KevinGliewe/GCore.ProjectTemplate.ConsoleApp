using System;
using Serilog;

namespace GCore.ProjectTemplate.ConsoleApp.Handler {
    public class InstancedHandler : IInstancedHandler
    {
        public void OnAction()
        {
            Log.Information("InstancedHandler::OnAction()");
        }
    }
}