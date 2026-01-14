using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UnoFramework.Contracts.Navigation;

namespace UnoFramework.Pages;

/// <summary>
/// Base page class that automatically triggers INavigationAware lifecycle on ViewModel.
/// Supports both Frame navigation and Region-based navigation (Uno Extensions).
/// </summary>
public class BasePage : Page
{
    private bool _hasNavigatedTo;
    private bool _isLoaded;

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
