using Linpress.Bootstrapper.Shell.Base;
using Linpress.Bootstrapper.Shell.Models;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

namespace Linpress.Bootstrapper.Shell.ViewModels
{
    public class InstallViewModel : BaseModel
    {
        #region 객체
        public UpdateState updateState;
        private InstallState installState;
        private int cacheProgress;
        private int executeProgress;
        private bool isUnstalling = false;
        private Dictionary<string, int> executingPackageOrderIndex;
        #endregion

        #region 속성
        public BootstrapperApplicationModel Model { get; set; }

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
        private string _Message = "Nothing";

        public InstallState State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value;

                if (State == InstallState.NotPresent)
                {
                    Description = "응용 프로그램을 설치할 준비가 되었습니다.\n계속하시면 최종 사용자 사용권 계약을 동의하고 설치를 시작합니다.";
                }
                else if (State == InstallState.Present)
                {
                    Description = "응용 프로그램을 제거할 준비가 되었습니다.\n계속하시면 장치에서 응용 프로그램이 완전히 제거됩니다.";
                }

                RaisePropertyChanged();
            }
        }
        private InstallState _State;

        public string PackageId
        {
            get
            {
                return _PackageId;
            }
            set
            {
                _PackageId = value;
                RaisePropertyChanged();
            }
        }
        private string _PackageId;

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
        private bool _Canceled = false;

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
                Model.SetBurnVariable("Username", Username);
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
                OnPropertyChanged("Persent");
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

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
                RaisePropertyChanged();
            }
        }
        private string _Description;
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
                return _RepairCommand ?? (_RepairCommand = new RelayCommand(param => Model.PlanAction(LaunchAction.Repair), param => State == InstallState.Present));
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

        #region 이벤트
        private void CacheComplete(object sender, CacheCompleteEventArgs e)
        {
            lock (this)
            {
                cacheProgress = 100;
                Progress = (cacheProgress + executeProgress) / Phases;
            }
        }

        private void CacheAcquireProgress(object sender, CacheAcquireProgressEventArgs e)
        {
            lock (this)
            {
                cacheProgress = e.OverallPercentage;
                Progress = (cacheProgress + executeProgress) / Phases;
                e.Result = Canceled ? Result.Cancel : Result.Ok;
            }
        }

        private void ExecuteProgress(object sender, ExecuteProgressEventArgs e)
        {
            lock (this)
            {
                executeProgress = e.OverallPercentage;
                Progress = (cacheProgress + executeProgress) / 2;

                if (BootstrapperProgram.Model.Command.Display == Display.Embedded)
                {
                    BootstrapperProgram.Model.Engine.SendEmbeddedProgress(e.ProgressPercentage, Progress);
                }

                e.Result = Canceled ? Result.Cancel : Result.Ok;
            }
        }

        private void ExecuteMsiMessage(object sender, ExecuteMsiMessageEventArgs e)
        {
            lock (this)
            {
                if (e.MessageType == InstallMessage.ActionStart)
                {
                    Message = e.Message;
                }

                e.Result = Canceled ? Result.Cancel : Result.Ok;
            }
        }

        private void PlanBegin(object sender, PlanBeginEventArgs e)
        {
            lock (this)
            {
                if (InstallEnabled)
                {
                    _Phases = (LaunchAction.Layout == BootstrapperProgram.Model.PlannedAction) ? 1 : 2;
                }
                else
                {
                    LabelBack = false;
                }

                InstallText = "";
                RepairText = "";
                OnPropertyChanged("Phases");
                OnPropertyChanged("InstallEnabled");
                OnPropertyChanged("InstallText");
                OnPropertyChanged("RepairText");
                executingPackageOrderIndex.Clear();
            }
        }

        private void PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (State == InstallState.Cancelled)
            {
                BootstrapperProgram.Dispatcher.InvokeShutdown();
                return;
            }

            State = InstallState.Applying;
            Model.ApplyAction();
        }

        private void PlanPackageComplete(object sender, PlanPackageCompleteEventArgs e)
        {
            if (ActionState.None != e.Execute)
            {
                lock (this)
                {
                    Debug.Assert(!executingPackageOrderIndex.ContainsKey(e.PackageId));
                    executingPackageOrderIndex.Add(e.PackageId, executingPackageOrderIndex.Count);
                }
            }
        }

        private void ApplyBegin(object sender, ApplyBeginEventArgs e)
        {
            State = InstallState.Applying;
            OnPropertyChanged("ProgressEnabled");
            OnPropertyChanged("CancelEnabled");
        }

        private void ApplyProgress(object sender, ProgressEventArgs e)
        {
            lock (this)
            {
                e.Result = Canceled ? Result.Cancel : Result.Ok;
            }
        }

        private void ApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            Model.FinalResult = e.Status;
            State = InstallState.Applied;
            isUnstalling = false;

            OnPropertyChanged("CompleteEnabled");
            OnPropertyChanged("ProgressEnabled");
        }

        private void ExecutePackageBegin(object sender, ExecutePackageBeginEventArgs e)
        {
            if (State == InstallState.Cancelled)
            {
                e.Result = Result.Cancel;
            }
        }

        private void ExecutePackageComplete(object sender, ExecutePackageCompleteEventArgs e)
        {
            if (State == InstallState.Cancelled)
            {
                e.Result = Result.Cancel;
            }
        }

        private void DetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            PackageId = e.PackageId;
            if (e.PackageId.Equals("DialogView", StringComparison.Ordinal))
            {
                State = e.State == PackageState.Present ? InstallState.Present : InstallState.NotPresent;
            }
        }
        #endregion

        #region 생성자
        public InstallViewModel(BootstrapperApplicationModel model)
        {
            Model = model;
            State = InstallState.Initializing;
            executingPackageOrderIndex = new Dictionary<string, int>();

            InstallCommand = new RelayCommand
            (
                param => Model.PlanAction(LaunchAction.Install),
                param => State == InstallState.NotPresent
            );

            UninstallCommand = new RelayCommand
            (
                param =>
                {
                    Model.PlanAction(LaunchAction.Uninstall);
                    isUnstalling = true;
                },
                param => State == InstallState.Present
            );

            CancelCommand = new RelayCommand
            (
                param =>
                {
                    Model.LogMessage("Cancelling...");
                    if (State == InstallState.Applying)
                    {
                        State = InstallState.Cancelled;
                    }
                    else
                    {
                        BootstrapperProgram.Dispatcher.InvokeShutdown();
                    }
                },
                param => State != InstallState.Cancelled
            );

            WireUpEventHandlers(Model);
        }
        #endregion

        #region 내부 함수
        private void WireUpEventHandlers(BootstrapperApplicationModel model)
        {
            model.Application.CacheComplete += CacheComplete;
            model.Application.CacheAcquireProgress += CacheAcquireProgress;

            model.Application.ExecuteProgress += ExecuteProgress;
            model.Application.ExecuteMsiMessage += ExecuteMsiMessage;

            model.Application.PlanBegin += PlanBegin;
            model.Application.PlanComplete += PlanComplete;
            model.Application.PlanPackageComplete += PlanPackageComplete;

            model.Application.ApplyBegin += ApplyBegin;
            model.Application.Progress += ApplyProgress;
            model.Application.ApplyComplete += ApplyComplete;

            model.Application.ExecutePackageBegin += ExecutePackageBegin;
            model.Application.ExecutePackageComplete += ExecutePackageComplete;

            model.Application.DetectPackageComplete += DetectPackageComplete;
        }
        #endregion
    }
}
