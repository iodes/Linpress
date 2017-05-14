using Linpress.Bootstrapper.Shell.Base;
using Linpress.Bootstrapper.Shell.Models;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Linpress.Bootstrapper.Shell.ViewModels
{
    public class InstallViewModel : BaseModel
    {
        #region 객체
        private Dictionary<string, int> executingPackageOrderIndex;
        private BootstrapperApplicationModel model;
        private bool isUnstalling = false;
        private InstallState installState;
        public UpdateState updateState;
        #endregion

        #region 속성
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
                RaisePropertyChanged();
            }
        }
        private string _Message;

        public InstallState State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value;
                Message = "Status: " + State;
                RaisePropertyChanged();
            }
        }
        private InstallState _State;

        public string PackageID
        {
            get
            {
                return _PackageID;
            }
            set
            {
                _PackageID = value;
                RaisePropertyChanged();
            }
        }
        private string _PackageID;

        public bool Canceled
        {
            get
            {
                return _Canceled;
            }
            set
            {
                Canceled = value;
                RaisePropertyChanged();
            }
        }
        private bool _Canceled;

        public Version Version
        {
            get
            {
                return _Version;
            }
        }
        private Version _Version = new Version("1.0.0.0");

        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
                model.SetBurnVariable("Username", Username);
            }
        }
        private string _Username;

        public int Progress
        {
            get
            {
                if (isUnstalling)
                {
                    return _Progress * 2;
                }

                return _Progress;
            }
            set
            {
                _Progress = value;
                RaisePropertyChanged();
                RaisePropertyChanged("Persent");
            }

        }
        private int _Progress;

        public string Info
        {
            get
            {
                if (string.IsNullOrEmpty(_Info))
                {
                    _Info = InstallEnabled ? "설치 중..." : "진행 중...";
                }

                return _Info;
            }
            set
            {
                _Info = value;
                RaisePropertyChanged();
            }
        }
        private string _Info;

        public string Percent
        {
            get { return Progress + "%"; }
        }

        public bool InstallEnabled
        {
            get { return State == InstallState.NotPresent; }
        }

        public bool UninstallEnabled
        {
            get { return UninstallCommand.CanExecute(this); }
        }

        public bool CancelEnabled
        {
            get { return State == InstallState.Applying; }
        }

        public bool ExitEnabled
        {
            get { return State != InstallState.Applying; }
        }

        public bool ProgressEnabled
        {
            get { return State == InstallState.Applying; }
        }

        public bool IsUpToDate
        {
            get { return true; }
        }
        public bool RepairEnabled
        {
            get { return RepairCommand.CanExecute(this); }
        }

        public bool CompleteEnabled
        {
            get { return State == InstallState.Applied; }
        }

        public int Phases
        {
            get
            {
                return _Phases;
            }
        }
        private int _Phases = 1;

        public string InstallText
        {
            get
            {
                return _InstallText;
            }
            set
            {
                _InstallText = value;
                RaisePropertyChanged();
            }
        }
        private string _InstallText = "Uninstall";

        public string RepairText
        {
            get
            {
                return _RepairText;
            }
            set
            {
                _RepairText = value;
            }
        }
        private string _RepairText = "Repair";

        public bool LabelBack
        {
            get
            {
                return _LabelBack;
            }
            set
            {
                _LabelBack = value;
                RaisePropertyChanged();
            }
        }
        private bool _LabelBack = true;
        #endregion

        #region 명령
        public ICommand InstallCommand { get; private set; }

        public ICommand UninstallCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }

        public ICommand LaunchNewsCommand { get; private set; }

        public ICommand RepairCommand
        {
            get
            {
                return _RepairCommand ?? (_RepairCommand = new RelayCommand(param => model.PlanAction(LaunchAction.Repair), param => State == InstallState.Present));
            }
        }
        private ICommand _RepairCommand;
        #endregion

        #region 열거형
        public enum InstallState
        {
            Initializing,
            Present,
            NotPresent,
            Applying,
            Cancelled,
            Applied,
            Failed,
        }

        public enum UpdateState
        {
            Unknown,
            Initializing,
            Checking,
            Current,
            Available,
            Failed,
        }
        #endregion
    }
}
