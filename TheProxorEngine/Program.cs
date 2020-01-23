using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using EngineEditor.GUI;

namespace TheProxorEngine
{
    class Program
    {
        private const string version = "0.1a";

        private static Window mainWindow;

        private static void Main(string[] args)
        {
            DoExecute();
        }

        private static void DoExecute()
        {
            mainWindow = new Window($"Proxor Engine {version}", 1920, 1080, Mode.WINDOWED);
            var task = Task.Factory.StartNew(mainWindow.Open);

            task.Wait();
        }
    }
}
