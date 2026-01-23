using Shiny.Mediator;

namespace UnoFramework.Contracts.Pages;

/// <summary>
/// Event published when navigation to a new page occurs.
/// Contains PageType, Title, and optional MainHeader ViewModel type.
/// </summary>
/// <param name="PageType">The type of the page (determines header icon)</param>
/// <param name="Title">The title to display</param>
/// <param name="MainHeaderViewModel">Optional: ViewModel type for MainHeader region</param>
public record PageNavigatedEvent(
    PageType PageType,
    string Title,
    Type? MainHeaderViewModel = null
) : IEvent;
