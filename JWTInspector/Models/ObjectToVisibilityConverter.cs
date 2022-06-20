using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace JWTInspector.Models;

[ValueConversion(typeof(object), typeof(Visibility))]
public class ObjectToVisibilityConverter : MarkupExtension, IValueConverter
{
    public Visibility NullValue { get; set; } = Visibility.Collapsed;
    public Visibility NonNullValue { get; set; } = Visibility.Visible;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!typeof(Visibility).IsAssignableFrom(targetType))
            throw new NotSupportedException("Converter can't be used to convert to isntances of type otehr than Visibility");
        return value == null ? NullValue : NonNullValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("Converter can't be used for bidirectional binding");
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
