using System;
using GCore.AppSystem.Handler;

namespace GCore.ProjectTemplate.ConsoleApp.Handler {

    [Handler("Handlers:InstancedHandler", nameof(InstancedHandler))]
    [Lifetime(Lifetime.Singleton)]
    public interface IInstancedHandler {
        void OnAction();
    }
}