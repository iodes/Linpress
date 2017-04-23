using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Linpress.Bootstrapper.Shell.Base
{
    public class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            if (name != null)
            {
                OnPropertyChanged(name);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {

        }
    }
}
