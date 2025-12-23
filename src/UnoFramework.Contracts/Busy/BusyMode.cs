namespace UnoFramework.Contracts.Busy;

/// <summary>
/// Specifies the busy mode for a command.
/// </summary>
public enum BusyMode
{
    /// <summary>No busy indicator.</summary>
    None = 0,
    /// <summary>Local busy indicator (ViewModel-level).</summary>
    Local = 1,
    /// <summary>Global busy indicator (App-level).</summary>
    Global = 2
}
