using Linpress.Bootstrapper.Shell.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Linpress.Bootstrapper.Shell.Views
{
    /// <summary>
    /// InstallView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InstallView : Window
    {
        public InstallView(InstallViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            Closed += (cS, cE) =>
            {
                viewModel.CancelCommand.Execute(viewModel);
            };
        }
    }
}
