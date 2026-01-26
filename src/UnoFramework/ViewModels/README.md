# UnoFramework ViewModels

Dieses Modul enthält die Basis-ViewModels für Uno Platform Anwendungen mit vollständiger Navigation Lifecycle-Unterstützung.

## Übersicht

Das ViewModel-System bietet drei Basis-Klassen:

1. **ViewModelBase** - Gemeinsame Basis für alle ViewModels
2. **PageViewModel** - Für Frame-basierte Page Navigation
3. **RegionViewModel** - Für Region-basierte Navigation (Uno Extensions)

## ViewModelBase

Die gemeinsame Basis für alle ViewModels mit folgenden Features:

### Properties
- `Logger` - ILogger Instanz für Logging
- `Mediator` - Shiny.Mediator für Event Publishing
- `Navigator` - INavigator für Navigation
- `RouteNotifier` - IRouteNotifier für Navigation Tracking
- `NavigationToken` - CancellationToken der bei Navigation cancelled wird
- `IsBusy` - Busy State Flag
- `BusyMessage` - Optionale Busy Message

### Lifecycle Methoden

```csharp
// Einmalige Initialisierung (lazy, beim ersten Navigieren)
protected virtual Task InitializeAsync(CancellationToken ct = default)
```

### Busy State Management

```csharp
// Manuell
SetBusy(true, "Loading...");
SetBusy(false);

// Automatisch mit using
using (BeginBusy("Loading..."))
{
    await LoadDataAsync();
}

// Global (publiziert GlobalBusyEvent)
using (BeginGlobalBusy("Saving..."))
{
    await SaveDataAsync();
}
```

## PageViewModel

Für **Frame-basierte Navigation** (Navigation zwischen Pages).

### Lifecycle

Nur zwei Methoden zum Überschreiben:

```csharp
// Wird bei jeder Navigation zur Page aufgerufen
protected virtual Task OnNavigatedToAsync(NavigationEventArgs e, CancellationToken ct = default)

// Wird beim Verlassen der Page aufgerufen
protected virtual Task OnNavigatedFromAsync(NavigationEventArgs e, CancellationToken ct = default)
```

### Verwendung

```csharp
public partial class MyPageViewModel : PageViewModel
{
    public MyPageViewModel(BaseServices baseServices) : base(baseServices)
    {
    }

    // Einmalige Initialisierung beim ersten Navigieren (lazy)
    protected override async Task InitializeAsync(CancellationToken ct = default)
    {
        await LoadStaticDataAsync(ct);
    }

    // Wird bei jeder Navigation zur Page aufgerufen
    protected override async Task OnNavigatedToAsync(NavigationEventArgs e, CancellationToken ct = default)
    {
        Logger.LogInformation("Navigated to page with parameter: {Parameter}", e.Parameter);
        await LoadDynamicDataAsync(ct);
    }

    // Wird beim Verlassen der Page aufgerufen (State speichern)
    protected override async Task OnNavigatedFromAsync(NavigationEventArgs e, CancellationToken ct = default)
    {
        await SaveStateAsync(ct);
    }

    private async Task LoadDynamicDataAsync(CancellationToken ct)
    {
        using (BeginBusy("Loading data..."))
        {
            // NavigationToken wird automatisch cancelled beim Wegnavigieren
            await Task.Delay(1000, NavigationToken);
        }
    }
}
```

### Page Setup

Die Page muss von `BasePage` erben:

```xml
<pages:BasePage
    x:Class="MyApp.MyPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:pages="using:UnoFramework.Pages">

    <!-- Page Content -->

</pages:BasePage>
```

```csharp
namespace MyApp;

public sealed partial class MyPage : BasePage
{
    public MyPage()
    {
        InitializeComponent();
    }
}
```

Die `BasePage` überschreibt `OnNavigatedTo` und `OnNavigatedFrom` und ruft automatisch die entsprechenden Methoden auf dem ViewModel auf.

**Wichtig:** `BasePage` behandelt korrekt den Fall, dass das `DataContext` (ViewModel) erst **nach** `OnNavigatedTo` gesetzt wird. Dies ist bei Uno Extensions Navigation üblich, wo das ViewModel über ViewMap dependency injection gesetzt wird. Die Navigation-Events werden zwischengespeichert und sobald das ViewModel gesetzt ist, wird `OnNavigatedToAsync` aufgerufen.

## RegionViewModel

Für **Region-basierte Navigation** (Uno Extensions Navigation mit Regions).

### Lifecycle

Nur zwei Methoden zum Überschreiben:

```csharp
// Wird aufgerufen wenn die Region geladen wird (Loaded)
protected virtual Task OnNavigatedToAsync(CancellationToken ct = default)

// Wird aufgerufen wenn die Region entladen wird (Unloaded)
protected virtual Task OnNavigatedFromAsync(CancellationToken ct = default)
```

### Verwendung

```csharp
public partial class MyRegionViewModel : RegionViewModel
{
    public MyRegionViewModel(BaseServices baseServices) : base(baseServices)
    {
    }

    // Einmalige Initialisierung
    protected override async Task InitializeAsync(CancellationToken ct = default)
    {
        await LoadStaticDataAsync(ct);
    }

    // Wird aufgerufen wenn zur Region navigiert wird (Control Loaded)
    protected override async Task OnNavigatedToAsync(CancellationToken ct = default)
    {
        Logger.LogInformation("Region activated");
        await LoadRegionDataAsync(ct);
    }

    // Wird aufgerufen wenn die Region verlassen wird (Control Unloaded)
    protected override async Task OnNavigatedFromAsync(CancellationToken ct = default)
    {
        await CleanupAsync(ct);
    }
}
```

### UserControl Setup

Der UserControl muss von `BaseRegionControl` erben:

```xml
<controls:BaseRegionControl
    x:Class="MyApp.MyRegionControl"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:controls="using:UnoFramework.Controls">

    <!-- Region Content -->

</controls:BaseRegionControl>
```

```csharp
namespace MyApp;

public sealed partial class MyRegionControl : BaseRegionControl
{
    public MyRegionControl()
    {
        InitializeComponent();
    }
}
```

Die `BaseRegionControl` ruft automatisch `OnNavigatedToAsync` (bei Loaded) und `OnNavigatedFromAsync` (bei Unloaded) auf dem ViewModel auf.

**Wichtig:** `BaseRegionControl` behandelt korrekt den Fall, dass das `DataContext` (ViewModel) erst **nach** `Loaded` gesetzt wird. Dies ist bei Uno Extensions Navigation üblich, wo das ViewModel über ViewMap dependency injection gesetzt wird. Die Lifecycle-Events werden zwischengespeichert und sobald das ViewModel gesetzt ist, wird `OnNavigatedToAsync` aufgerufen.

### Alternative: Manueller Aufruf

Wenn du nicht von `BaseRegionControl` erben möchtest, kannst du die Lifecycle-Methoden manuell aufrufen:

```csharp
public sealed partial class MyRegionControl : UserControl
{
    public MyRegionControl()
    {
        InitializeComponent();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegionViewModel viewModel)
        {
            await viewModel.NotifyNavigatedToAsync();
        }
    }

    private async void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegionViewModel viewModel)
        {
            await viewModel.NotifyNavigatedFromAsync();
        }
    }
}
```

## Navigation Data über Constructor Injection

Daten werden über den Konstruktor injiziert:

```csharp
public record UserDetailsData(Guid UserId, string UserName);

public partial class UserDetailsViewModel : PageViewModel
{
    private readonly UserDetailsData _data;

    public UserDetailsViewModel(
        BaseServices baseServices,
        UserDetailsData data) : base(baseServices)
    {
        _data = data;
    }

    protected override async Task OnNavigatedToAsync(NavigationEventArgs e, CancellationToken ct = default)
    {
        // Verwende _data um User zu laden
        await LoadUserAsync(_data.UserId, ct);
    }
}
```

Navigation mit Daten:

```csharp
var data = new UserDetailsData(userId, userName);
await Navigator.NavigateViewModelAsync<UserDetailsViewModel>(this, data: data);
```

## Best Practices

### 1. InitializeAsync vs OnNavigatedToAsync

- **InitializeAsync**: Einmalige Initialisierung beim ersten Navigieren (lazy)
  - Statische Daten laden
  - Subscriptions einrichten
  - Services initialisieren
  - Wird von `BasePage`/`BaseRegionControl` automatisch beim ersten `OnNavigatedToAsync` ausgelöst

- **OnNavigatedToAsync**: Bei jeder Navigation zur Page/Region
  - Dynamische Daten laden/aktualisieren
  - Parameter verarbeiten
  - UI State aktualisieren

**Hinweis:** `OnNavigatedTo` wird vor dem Visual Tree aufgerufen. UI-spezifische Manipulationen sollten in `Loaded` oder über Bindings/VisualStates erfolgen.

### 2. NavigationToken verwenden

Alle async Operationen sollten `NavigationToken` verwenden:

```csharp
protected override async Task OnNavigatedToAsync(NavigationEventArgs e, CancellationToken ct = default)
{
    // RICHTIG: Wird automatisch cancelled beim Wegnavigieren
    await LoadDataAsync(NavigationToken);

    // FALSCH: Läuft weiter auch nach Navigation
    await LoadDataAsync(CancellationToken.None);
}
```

### 3. Busy State Management

```csharp
// Lokal (nur im ViewModel)
using (BeginBusy("Loading..."))
{
    await LoadDataAsync(NavigationToken);
}

// Global (für Loading Overlay in Shell)
using (BeginGlobalBusy("Saving..."))
{
    await SaveDataAsync(NavigationToken);
}
```

### 4. Exception Handling

```csharp
protected override async Task OnNavigatedToAsync(NavigationEventArgs e, CancellationToken ct = default)
{
    try
    {
        using (BeginBusy("Loading..."))
        {
            await LoadDataAsync(NavigationToken);
        }
    }
    catch (OperationCanceledException)
    {
        // Navigation cancelled - normal, nichts tun
    }
    catch (Exception ex)
    {
        Logger.LogError(ex, "Failed to load data");
        // Fehler anzeigen
    }
}
```

### 5. Cleanup in OnNavigatedFromAsync

```csharp
protected override async Task OnNavigatedFromAsync(NavigationEventArgs e, CancellationToken ct = default)
{
    // State speichern
    // Resources freigeben
    // Subscriptions aufräumen
    // Timers stoppen
    await SaveStateAsync(ct);
}
```

## Beispiele

### PageViewModel mit Mediator Commands

```csharp
[Service(UnoService.Lifetime, TryAdd = UnoService.TryAdd)]
public partial class HomeViewModel : PageViewModel
{
    [ObservableProperty]
    private string _welcomeMessage = string.Empty;

    public HomeViewModel(BaseServices baseServices) : base(baseServices)
    {
    }

    protected override async Task OnNavigatedToAsync(NavigationEventArgs e, CancellationToken ct = default)
    {
        using (BeginBusy("Loading..."))
        {
            var response = await Mediator.Request(new GetWelcomeMessageRequest(), ct);
            WelcomeMessage = response.Message;
        }
    }

    [RelayCommand]
    private async Task NavigateToDetailsAsync(Guid itemId)
    {
        await Navigator.NavigateViewModelAsync<DetailsViewModel>(
            this,
            data: new DetailsData(itemId));
    }
}
```

### RegionViewModel mit IRouteNotifier

```csharp
[Service(UnoService.Lifetime, TryAdd = UnoService.TryAdd)]
public partial class SidebarViewModel : RegionViewModel
{
    [ObservableProperty]
    private ObservableCollection<MenuItem> _menuItems = [];

    public SidebarViewModel(BaseServices baseServices) : base(baseServices)
    {
    }

    protected override async Task InitializeAsync(CancellationToken ct = default)
    {
        // Einmalig: Menu Items laden
        var response = await Mediator.Request(new GetMenuItemsRequest(), ct);
        MenuItems = new ObservableCollection<MenuItem>(response.Items);
    }

    protected override async Task OnNavigatedToAsync(CancellationToken ct = default)
    {
        Logger.LogInformation("Sidebar region activated");
        // Dynamische Updates durchführen
    }
}
```

## Siehe auch

- [Uno Platform Navigation Documentation](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Navigation/NavigationOverview.html)
- [IRouteNotifier How-To](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Navigation/Advanced/HowTo-IRouteNotifier.html)
- [Shiny.Mediator Documentation](https://github.com/shinyorg/mediator)
