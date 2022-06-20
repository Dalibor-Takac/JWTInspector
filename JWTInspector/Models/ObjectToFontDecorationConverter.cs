using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace JWTInspector.Models;

[ValueConversion(typeof(string), typeof(TextDecorationCollection))]
public class ObjectToFontDecorationConverter : MarkupExtension, IValueConverter
{
    public TextDecorationCollection? NullValue { get; set; } = null;
    public TextDecorationCollection? NonNullValue { get; set; } = TextDecorations.Underline;

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!typeof(TextDecorationCollection).IsAssignableFrom(targetType))
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
