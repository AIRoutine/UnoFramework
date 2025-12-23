using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace UnoFramework.Controls;

/// <summary>
/// A control that displays a busy indicator overlay with an optional message.
/// </summary>
public partial class BusyOverlay : ContentControl
{
    public BusyOverlay()
    {
        DefaultStyleKey = typeof(BusyOverlay);
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;
    }

    #region IsBusy DependencyProperty

    public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
        nameof(IsBusy),
        typeof(bool),
        typeof(BusyOverlay),
        new PropertyMetadata(false, OnIsBusyChanged));

    /// <summary>
    /// Gets or sets whether the busy indicator is visible.
    /// </summary>
    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    private static void OnIsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BusyOverlay overlay)
        {
            overlay.UpdateVisualState();
        }
    }

    #endregion

    #region BusyMessage DependencyProperty

    public static readonly DependencyProperty BusyMessageProperty = DependencyProperty.Register(
        nameof(BusyMessage),
        typeof(string),
        typeof(BusyOverlay),
        new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the message to display while busy.
    /// </summary>
    public string? BusyMessage
    {
        get => (string?)GetValue(BusyMessageProperty);
        set => SetValue(BusyMessageProperty, value);
    }

    #endregion

    #region OverlayBackground DependencyProperty

    public static readonly DependencyProperty OverlayBackgroundProperty = DependencyProperty.Register(
        nameof(OverlayBackground),
        typeof(Brush),
        typeof(BusyOverlay),
        new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the background brush for the overlay.
    /// </summary>
    public Brush? OverlayBackground
    {
        get => (Brush?)GetValue(OverlayBackgroundProperty);
        set => SetValue(OverlayBackgroundProperty, value);
    }

    #endregion

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        VisualStateManager.GoToState(this, IsBusy ? "Busy" : "NotBusy", true);
    }
}
