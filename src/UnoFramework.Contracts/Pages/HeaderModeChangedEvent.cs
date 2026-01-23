using Shiny.Mediator;

namespace UnoFramework.Contracts.Pages;

/// <summary>
/// Event published when the header mode changes (Menu/Normal)
/// </summary>
/// <param name="Mode">The new header mode</param>
public record HeaderModeChangedEvent(HeaderMode Mode) : IEvent;
