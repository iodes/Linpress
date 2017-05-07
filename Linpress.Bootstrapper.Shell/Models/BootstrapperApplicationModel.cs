using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Windows;
using System.Windows.Interop;

namespace Linpress.Bootstrapper.Shell.Models
{
    public class BootstrapperApplicationModel
    {
        #region 객체
        private IntPtr hwnd;
        #endregion

        #region 속성
        public int FinalResult { get; set; }

        public BootstrapperApplication Application { get; private set; }
        #endregion

        #region 생성자
        public BootstrapperApplicationModel(BootstrapperApplication application)
        {
            Application = application;
            hwnd = IntPtr.Zero;
        }
        #endregion

        #region 사용자 함수
        public void PlanAction(LaunchAction action)
        {
            Application.Engine.Plan(action);
        }

        public void ApplyAction()
        {
            Application.Engine.Apply(hwnd);
        }

        public void LogMessage(string message)
        {
            Application.Engine.Log(LogLevel.Standard, message);
        }

        public void SetBurnVariable(string variableName, string value)
        {
            Application.Engine.StringVariables[variableName] = value;
        }

        public void SetWindowHandle(Window view)
        {
            hwnd = new WindowInteropHelper(view).Handle;
        }
        #endregion
    }
}
