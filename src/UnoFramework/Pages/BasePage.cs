using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Shiny.Mediator;
using UnoFramework.Contracts.Application;
using UnoFramework.Contracts.Navigation;
using UnoFramework.Contracts.Pages;

namespace UnoFramework.Pages;

/// <summary>
/// Base page class that automatically triggers INavigationAware lifecycle on ViewModel.
/// Supports both Frame navigation and Region-based navigation (Uno Extensions).
/// </summary>
public class BasePage : Page
{
    private bool _hasNavigatedTo;
    private bool _isLoaded;

    /// <summary>
    /// Identifies the HeaderMode dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderModeProperty =
        DependencyProperty.Register(
            nameof(HeaderMode),
            typeof(HeaderMode),
            typeof(BasePage),
            new PropertyMetadata(HeaderMode.Menu, OnHeaderModeChanged));

    /// <summary>
    /// Gets or sets the header mode (Menu/Normal) for this page.
    /// Menu mode shows a hamburger button, Normal mode shows a back button.
    /// </summary>
    public HeaderMode HeaderMode
    {
        get => (HeaderMode)GetValue(HeaderModeProperty);
        set => SetValue(HeaderModeProperty, value);
    }

    private static void OnHeaderModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BasePage page && page._isLoaded)
        {
            page.PublishHeaderModeChanged((HeaderMode)e.NewValue);
        }
    }

    private void PublishHeaderModeChanged(HeaderMode mode)
    {
        try
        {
            // Get mediator from application services
            var serviceProvider = Application.Current is IApplicationWithServices appWithServices
                ? appWithServices.Services
                : null;

            var mediator = serviceProvider?.GetService<IMediator>();
            mediator?.Publish(new HeaderModeChangedEvent(mode));
        }
        catch
        {
            // Silently ignore if mediator not available
        }
    }

    public BasePage()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        DataContextChanged += OnDataContextChanged;
    }

    /// <summary>
    /// Handles DataContextChanged - triggers OnNavigatedTo when DataContext is set after Loaded.
    /// </summary>
    private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        TryTriggerNavigatedTo();
    }

    /// <summary>
    /// Handles Loaded event for Region-based navigation (Uno Extensions).
    /// Frame navigation uses OnNavigatedTo instead.
    /// </summary>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = true;
        PublishHeaderModeChanged(HeaderMode);
        TryTriggerNavigatedTo();
    }

    /// <summary>
    /// Attempts to trigger OnNavigatedTo if conditions are met:
    /// - Page is loaded
    /// - DataContext is INavigationAware
    /// - OnNavigatedTo hasn't been called yet
    /// </summary>
    private void TryTriggerNavigatedTo()
    {
        if (_isLoaded && !_hasNavigatedTo && DataContext is INavigationAware navigationAware)
        {
            _hasNavigatedTo = true;
            navigationAware.OnNavigatedTo(null);
        }
    }

    /// <summary>
    /// Handles Unloaded event for Region-based navigation cleanup.
    /// </summary>
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = false;
        if (_hasNavigatedTo && DataContext is INavigationAware navigationAware)
        {
            navigationAware.OnNavigatedFrom();
            _hasNavigatedTo = false;
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // Frame navigation - mark as navigated and trigger ViewModel
        if (!_hasNavigatedTo && DataContext is INavigationAware navigationAware)
        {
            _hasNavigatedTo = true;
            navigationAware.OnNavigatedTo(e.Parameter);
        }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);

        if (_hasNavigatedTo && DataContext is INavigationAware navigationAware)
        {
            navigationAware.OnNavigatedFrom();
            _hasNavigatedTo = false;
        }
    }
}
