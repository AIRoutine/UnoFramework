using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Shiny.Mediator;
using Shiny.Mediator.Infrastructure;
using Uno.Extensions.Navigation;

namespace UnoFramework.Mediator;

/// <summary>
/// Collects event handlers from Uno Platform's visual tree AND DI container for Shiny.Mediator.
/// Supports both view/ViewModel-based handlers and explicitly registered IEventHandler implementations.
/// </summary>
[Service(UnoFrameworkService.Lifetime, TryAdd = UnoFrameworkService.TryAdd)]
public class UnoEventCollector : IEventCollector
{
    readonly IServiceProvider _serviceProvider;
    readonly ILogger<UnoEventCollector> _logger;
    readonly List<WeakReference<FrameworkElement>> _trackedViews = [];
    readonly object _lock = new();

    public UnoEventCollector(IServiceProvider serviceProvider, IRouteNotifier routeNotifier, ILogger<UnoEventCollector> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        routeNotifier.RouteChanged += OnRouteChanged;
    }

    void OnRouteChanged(object? sender, RouteChangedEventArgs e)
    {
        var view = e.Region?.View;
        if (view is null)
            return;

        lock (_lock)
        {
            CleanupDeadReferences();

            if (!IsAlreadyTracked(view))
            {
                _trackedViews.Add(new WeakReference<FrameworkElement>(view));
                _logger.LogDebug("Tracking view: {ViewType}, total tracked: {Count}",
                    view.GetType().Name, _trackedViews.Count);
            }
        }
    }

    public IReadOnlyList<IEventHandler<TEvent>> GetHandlers<TEvent>() where TEvent : IEvent
    {
        _logger.LogDebug("Collecting event handlers for Event Type: {Type}",
            typeof(TEvent).FullName);

        var handlers = new List<IEventHandler<TEvent>>();
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

        // First, collect handlers explicitly registered in DI
        var diHandlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();
        foreach (var handler in diHandlers)
        {
            if (visited.Add(handler))
            {
                handlers.Add(handler);
                _logger.LogDebug("Found DI-registered handler: {HandlerType}", handler.GetType().Name);
            }
        }

        // Then, collect handlers from the visual tree
        lock (_lock)
        {
            CleanupDeadReferences();

            foreach (var weakRef in _trackedViews)
            {
                if (!weakRef.TryGetTarget(out var view))
                    continue;

                // Collect handlers from visual tree (downwards)
                CollectHandlersFromVisualTree(view, handlers, visited);

                // Also collect handlers from parent chain (upwards) - this finds shell/container pages like MainPage
                CollectHandlersFromParentChain(view, handlers, visited);
            }
        }

        _logger.LogDebug("Found {Count} total handlers for Event Type: {Type}",
            handlers.Count, typeof(TEvent).FullName);

        return handlers;
    }

    void CollectHandlersFromVisualTree<TEvent>(DependencyObject element, List<IEventHandler<TEvent>> handlers, HashSet<object> visited) where TEvent : IEvent
    {
        if (element is null || !visited.Add(element))
            return;

        // Check if the element itself is a handler (element was just added to visited, so no need to check again)
        if (element is IEventHandler<TEvent> viewHandler)
        {
            handlers.Add(viewHandler);
            _logger.LogDebug("Found handler in view element: {HandlerType}", viewHandler.GetType().Name);
        }

        // Check if the element's DataContext is a handler (DataContext is a different object, so check visited)
        if (element is FrameworkElement fe && fe.DataContext is IEventHandler<TEvent> vmHandler && visited.Add(vmHandler))
        {
            handlers.Add(vmHandler);
            _logger.LogDebug("Found handler in DataContext of {ViewType}: {HandlerType}",
                fe.GetType().Name, vmHandler.GetType().Name);
        }

        // Traverse ContentControl.Content (e.g., Frame, ContentControl, ExtendedSplashScreen)
        if (element is ContentControl cc && cc.Content is DependencyObject content)
        {
            CollectHandlersFromVisualTree(content, handlers, visited);
        }

        // Traverse visual children
        var childCount = VisualTreeHelper.GetChildrenCount(element);
        for (var i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            CollectHandlersFromVisualTree(child, handlers, visited);
        }
    }

    /// <summary>
    /// Traverses the visual tree upwards to find handlers in parent elements.
    /// This is essential for finding handlers in shell/container pages like MainPage
    /// that host navigation content regions.
    /// </summary>
    void CollectHandlersFromParentChain<TEvent>(DependencyObject element, List<IEventHandler<TEvent>> handlers, HashSet<object> visited) where TEvent : IEvent
    {
        var parent = VisualTreeHelper.GetParent(element);

        while (parent is not null)
        {
            // Skip if we've already visited this parent (prevents duplicates)
            if (!visited.Add(parent))
            {
                parent = VisualTreeHelper.GetParent(parent);
                continue;
            }

            // Check if the parent element itself is a handler
            if (parent is IEventHandler<TEvent> viewHandler)
            {
                handlers.Add(viewHandler);
                _logger.LogDebug("Found handler in parent element: {HandlerType}", viewHandler.GetType().Name);
            }

            // Check if the parent element's DataContext is a handler
            if (parent is FrameworkElement fe && fe.DataContext is IEventHandler<TEvent> vmHandler && visited.Add(vmHandler))
            {
                handlers.Add(vmHandler);
                _logger.LogDebug("Found handler in parent DataContext of {ViewType}: {HandlerType}",
                    fe.GetType().Name, vmHandler.GetType().Name);
            }

            parent = VisualTreeHelper.GetParent(parent);
        }
    }

    void CleanupDeadReferences()
    {
        _trackedViews.RemoveAll(wr => !wr.TryGetTarget(out _));
    }

    bool IsAlreadyTracked(FrameworkElement view)
    {
        foreach (var wr in _trackedViews)
        {
            if (wr.TryGetTarget(out var v) && ReferenceEquals(v, view))
                return true;
        }
        return false;
    }
}
