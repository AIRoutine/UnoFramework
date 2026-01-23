namespace UnoFramework.Contracts.Pages;

/// <summary>
/// Interface for ViewModels that provide page information.
/// Used by BasePage to publish PageNavigatedEvent.
/// </summary>
public interface IPageInfo
{
    /// <summary>
    /// The type of the page - determines the header icon (hamburger vs back).
    /// </summary>
    PageType PageType { get; }

    /// <summary>
    /// The title to display in the header.
    /// </summary>
    string PageTitle { get; }

    /// <summary>
    /// Optional: ViewModel type for the MainHeader region.
    /// If null, the MainHeader region stays empty.
    /// </summary>
    Type? MainHeaderViewModel => null;
}
