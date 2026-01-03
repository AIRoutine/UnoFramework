using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Shiny.Mediator;
using Shiny.Mediator.Infrastructure;
using Uno.Extensions.Navigation;

namespace UnoFramework.Mediator;

/// <summary>
/// Collects event handlers from Uno Platform's visual tree for Shiny.Mediator.
/// </summary>
[Service(UnoFrameworkService.Lifetime, TryAdd = UnoFrameworkService.TryAdd)]
public class UnoEventCollector : IEventCollector
{
    readonly ILogger<UnoEventCollector> _logger;
    readonly List<WeakReference<FrameworkElement>> _trackedViews = [];
    readonly object _lock = new();

    public UnoEventCollector(IRouteNotifier routeNotifier, ILogger<UnoEventCollector> logger)
    {
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
        _logger.LogDebug("Collecting Uno views/binding contexts for Event Handler Type: {Type}",
            typeof(TEvent).FullName);

        var handlers = new List<IEventHandler<TEvent>>();
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

        lock (_lock)
        {
            CleanupDeadReferences();

            foreach (var weakRef in _trackedViews)
            {
                if (!weakRef.TryGetTarget(out var view))
                    continue;

                CollectHandlersFromVisualTree(view, handlers, visited);
            }
        }

        _logger.LogDebug("Found {Count} Uno views/binding contexts for Event Handler Type: {Type}",
            handlers.Count, typeof(TEvent).FullName);

        return handlers;
    }

    void CollectHandlersFromVisualTree<TEvent>(DependencyObject element, List<IEventHandler<TEvent>> handlers, HashSet<object> visited) where TEvent : IEvent
    {
        if (element is null || !visited.Add(element))
            return;

        // Check if the element itself is a handler
        if (element is IEventHandler<TEvent> viewHandler && visited.Add(viewHandler))
            handlers.Add(viewHandler);

        // Check if the element's DataContext is a handler
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
