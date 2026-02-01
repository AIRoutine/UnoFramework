namespace UnoFramework.Controls;

/// <summary>
/// Fluent extension methods for applying responsive values to AutoLayout properties.
/// </summary>
public static class ResponsiveAutoLayoutExtensions
{
    /// <summary>
    /// Applies responsive Padding values to AutoLayout based on screen size breakpoints.
    /// </summary>
    /// <param name="element">The AutoLayout element.</param>
    /// <param name="narrowest">Padding for narrowest screens (0-149px). Optional.</param>
    /// <param name="narrow">Padding for narrow screens (150-599px). Optional.</param>
    /// <param name="normal">Padding for normal screens (600-799px). Optional.</param>
    /// <param name="wide">Padding for wide screens (800-1079px). Optional.</param>
    /// <param name="widest">Padding for widest screens (1080px+). Optional.</param>
    /// <returns>The AutoLayout element for fluent chaining.</returns>
    public static AutoLayout ResponsivePadding(
        this AutoLayout element,
        double? narrowest = null,
        double? narrow = null,
        double? normal = null,
        double? wide = null,
        double? widest = null)
    {
        var responsive = new ResponsiveExtension();

        if (narrowest.HasValue)
        {
            responsive.Narrowest = narrowest.Value;
        }

        if (narrow.HasValue)
        {
            responsive.Narrow = narrow.Value;
        }

        if (normal.HasValue)
        {
            responsive.Normal = normal.Value;
        }

        if (wide.HasValue)
        {
            responsive.Wide = wide.Value;
        }

        if (widest.HasValue)
        {
            responsive.Widest = widest.Value;
        }

        _ = ResponsiveExtension.Install(element, typeof(AutoLayout), nameof(AutoLayout.Padding), responsive);
        return element;
    }

    /// <summary>
    /// Applies responsive Spacing values to AutoLayout based on screen size breakpoints.
    /// </summary>
    /// <param name="element">The AutoLayout element.</param>
    /// <param name="narrowest">Spacing for narrowest screens (0-149px). Optional.</param>
    /// <param name="narrow">Spacing for narrow screens (150-599px). Optional.</param>
    /// <param name="normal">Spacing for normal screens (600-799px). Optional.</param>
    /// <param name="wide">Spacing for wide screens (800-1079px). Optional.</param>
    /// <param name="widest">Spacing for widest screens (1080px+). Optional.</param>
    /// <returns>The AutoLayout element for fluent chaining.</returns>
    public static AutoLayout ResponsiveSpacing(
        this AutoLayout element,
        double? narrowest = null,
        double? narrow = null,
        double? normal = null,
        double? wide = null,
        double? widest = null)
    {
        var responsive = new ResponsiveExtension();

        if (narrowest.HasValue)
        {
            responsive.Narrowest = narrowest.Value;
        }

        if (narrow.HasValue)
        {
            responsive.Narrow = narrow.Value;
        }

        if (normal.HasValue)
        {
            responsive.Normal = normal.Value;
        }

        if (wide.HasValue)
        {
            responsive.Wide = wide.Value;
        }

        if (widest.HasValue)
        {
            responsive.Widest = widest.Value;
        }

        _ = ResponsiveExtension.Install(element, typeof(AutoLayout), nameof(AutoLayout.Spacing), responsive);
        return element;
    }
}
