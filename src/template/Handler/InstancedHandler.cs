using System;
using Anotar.Serilog;

namespace GCore.ProjectTemplate.ConsoleApp.Handler {
    public class InstancedHandler : IInstancedHandler
    {
        public void OnAction()
        {
            LogTo.Information("InstancedHandler::OnAction()");
        }
    }
}