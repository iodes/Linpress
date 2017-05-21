using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Linpress.Bootstrapper.Shell.Base
{
    public abstract class BaseValueConverter<TFrom, TTo> : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((TFrom)value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((TTo)value, parameter);
        }

        public abstract TTo Convert(TFrom value, object parameter);

        public abstract TFrom ConvertBack(TTo value, object parameter);
    }
}
