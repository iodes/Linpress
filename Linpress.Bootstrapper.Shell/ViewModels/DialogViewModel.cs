using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Linpress.Bootstrapper.Shell.ViewModels
{
    public class DialogViewModel
    {
        private const string BurnBundleInstallDirectoryVariable = "InstallFolder";
        private const string BurnBundleLayoutDirectoryVariable = "WixBundleLayoutDirectory";

        public DialogViewModel(BootstrapperApplication bootstrapper)
        {
            Bootstrapper = bootstrapper;
            Telemetry = new List<KeyValuePair<string, string>>();
        }

        public BootstrapperApplication Bootstrapper { get; private set; }

        public Command Command { get { return Bootstrapper.Command; } }

        public Engine Engine { get { return Bootstrapper.Engine; } }

        public List<KeyValuePair<string, string>> Telemetry { get; private set; }

        public int Result { get; set; }

        public Version Version
        {
            get
            {
                if (null == _Version)
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);

                    _Version = new Version(fileVersion.FileVersion);
                }

                return _Version;
            }
        }
        private Version _Version;

        public string InstallDirectory
        {
            get
            {
                if (!Engine.StringVariables.Contains(BurnBundleInstallDirectoryVariable))
                {
                    return null;
                }

                return Engine.StringVariables[BurnBundleInstallDirectoryVariable];
            }
            set
            {
                Engine.StringVariables[BurnBundleInstallDirectoryVariable] = value;
            }
        }

        public string LayoutDirectory
        {
            get
            {
                if (!Engine.StringVariables.Contains(BurnBundleLayoutDirectoryVariable))
                {
                    return null;
                }

                return Engine.StringVariables[BurnBundleLayoutDirectoryVariable];
            }
            set
            {
                Engine.StringVariables[BurnBundleLayoutDirectoryVariable] = value;
            }
        }

        public LaunchAction PlannedAction { get; set; }
    }
}
