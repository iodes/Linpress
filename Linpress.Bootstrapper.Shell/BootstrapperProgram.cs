using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Linpress.Bootstrapper.Shell
{
    public class BootstrapperProgram : BootstrapperApplication
    {
        protected override void Run()
        {
            MessageBox.Show("설치 프로그램 시작");
            Engine.Quit(0);
        }
    }
}
