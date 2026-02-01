using Microsoft.UI.Xaml.Data;

namespace UnoFramework.Controls;

/// <summary>
/// Fluent extension methods for BusyOverlay control.
/// </summary>
public static class BusyOverlayExtensions
{
    /// <summary>
    /// Sets the IsBusy property with a binding expression.
    /// </summary>
    public static BusyOverlay IsBusy(this BusyOverlay element, Func<bool> binding)
    {
        ArgumentNullException.ThrowIfNull(element);
        ArgumentNullException.ThrowIfNull(binding);

        var bindingExpression = new Binding
        {
            Path = new Microsoft.UI.Xaml.PropertyPath(binding.GetBindingPath()),
            Mode = BindingMode.OneWay
        };
        element.SetBinding(BusyOverlay.IsBusyProperty, bindingExpression);
        return element;
    }

    /// <summary>
    /// Sets the BusyMessage property with a binding expression.
    /// </summary>
    public static BusyOverlay BusyMessage(this BusyOverlay element, Func<string?> binding)
    {
        ArgumentNullException.ThrowIfNull(element);
        ArgumentNullException.ThrowIfNull(binding);

        var bindingExpression = new Binding
        {
            Path = new Microsoft.UI.Xaml.PropertyPath(binding.GetBindingPath()),
            Mode = BindingMode.OneWay
        };
        element.SetBinding(BusyOverlay.BusyMessageProperty, bindingExpression);
        return element;
    }

    /// <summary>
    /// Helper method to extract property path from lambda expression.
    /// </summary>
    private static string GetBindingPath(this Delegate binding)
    {
        if (binding.Target is null)
        {
            return string.Empty;
        }

        var propertyName = binding.Method.Name;

        // Handle compiler-generated property getters (e.g., get_PropertyName)
        if (propertyName.StartsWith("get_", StringComparison.Ordinal))
        {
            propertyName = propertyName[4..];
        }

        return propertyName;
    }
}
