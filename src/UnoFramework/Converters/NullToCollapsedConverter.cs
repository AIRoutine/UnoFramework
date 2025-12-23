using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace UnoFramework.Converters;

/// <summary>
/// Converts null or empty string values to Visibility.Collapsed, and non-null values to Visibility.Visible.
/// </summary>
public class NullToCollapsedConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string? language)
    {
        if (value == null)
        {
            return Visibility.Collapsed;
        }

        if (value is string str && string.IsNullOrWhiteSpace(str))
        {
            return Visibility.Collapsed;
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, string? language)
    {
        throw new NotImplementedException();
    }
}
