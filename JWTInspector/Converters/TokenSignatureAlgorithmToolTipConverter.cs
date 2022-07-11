using JWTInspector.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace JWTInspector.Converters;

[ValueConversion(typeof(TokenSignatureAlgorithm), typeof(string))]
public class TokenSignatureAlgorithmToolTipConverter : MarkupExtension, IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(string) && targetType != typeof(object))
            throw new NotSupportedException("Converter can't be used to convert to intances of type other than string or object");

        if (value is not TokenSignatureAlgorithm)
            throw new NotSupportedException("Converter can't be used to convert from anything else but instance of TokenSignatureAlgorithm");

        var enumField = typeof(TokenSignatureAlgorithm).GetField(Enum.GetName((TokenSignatureAlgorithm)value)!);
        var descriptionAttribute = enumField?.GetCustomAttribute<DescriptionAttribute>();

        return descriptionAttribute is not null ? descriptionAttribute.Description : null;
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
