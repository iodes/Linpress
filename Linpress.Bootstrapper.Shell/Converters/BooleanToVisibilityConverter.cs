using Linpress.Bootstrapper.Shell.Base;
using System.Windows;

namespace Linpress.Bootstrapper.Shell.Converters
{
    public class BooleanToVisibilityConverter : BaseValueConverter<bool, Visibility>
    {
        public override Visibility Convert(bool value, object parameter)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        public override bool ConvertBack(Visibility value, object parameter)
        {
            if (value == Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
