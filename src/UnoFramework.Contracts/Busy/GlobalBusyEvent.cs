using Shiny.Mediator;

namespace UnoFramework.Contracts.Busy;

/// <summary>
/// Event to set or clear the global busy state.
/// </summary>
/// <param name="IsBusy">Whether the app is busy.</param>
/// <param name="Message">Optional message to display.</param>
public record GlobalBusyEvent(bool IsBusy, string? Message = null) : IEvent;
