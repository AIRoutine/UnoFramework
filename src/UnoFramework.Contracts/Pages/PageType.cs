namespace UnoFramework.Contracts.Pages;

/// <summary>
/// Defines the type of a page for header icon and behavior.
/// </summary>
public enum PageType
{
    /// <summary>
    /// Home page - shows hamburger button
    /// </summary>
    Home,

    /// <summary>
    /// List page (Favorites, My Properties, etc.) - shows hamburger button
    /// </summary>
    List,

    /// <summary>
    /// Detail page - shows back button
    /// </summary>
    Detail,

    /// <summary>
    /// Form page (Add, Edit) - shows back button
    /// </summary>
    Form,

    /// <summary>
    /// Settings page - shows hamburger button
    /// </summary>
    Settings
}
