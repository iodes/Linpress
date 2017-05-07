using Linpress.Bootstrapper.Shell.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linpress.Bootstrapper.Shell.ViewModels
{
    public class InstallViewModel : BaseModel
    {
        #region 객체
        private InstallState installState;
        public UpdateState updateState;
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
