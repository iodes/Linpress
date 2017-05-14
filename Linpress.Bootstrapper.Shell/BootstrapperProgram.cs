using Linpress.Bootstrapper.Shell.Models;
using Linpress.Bootstrapper.Shell.ViewModels;
using Linpress.Bootstrapper.Shell.Views;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Windows.Threading;

namespace Linpress.Bootstrapper.Shell
{
    public class BootstrapperProgram : BootstrapperApplication
    {
        static public InstallView View { get; private set; }

        static public DialogViewModel Model { get; private set; }

        public static Dispatcher Dispatcher { get; set; }

        protected override void Run()
        {
            Model = new DialogViewModel(this);
            Dispatcher = Dispatcher.CurrentDispatcher;

            var model = new BootstrapperApplicationModel(this);
            var viewModel = new InstallViewModel(model);
            View = new InstallView(viewModel);
            model.SetWindowHandle(View);

            Engine.Detect();
            View.Show();
            Dispatcher.Run();

            Engine.Quit(model.FinalResult);
        }
    }
}
