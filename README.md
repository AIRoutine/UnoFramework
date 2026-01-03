# UnoFramework

A framework for building Uno Platform applications with Shiny Mediator, UnoCommand pattern, and Regions support.

## Projects

| Project | Description |
|---------|-------------|
| **UnoFramework** | Main framework with ViewModels, Controls, Busy patterns |
| **UnoFramework.Contracts** | Interfaces, events, and enums |
| **UnoFramework.Generators** | UnoCommand source generator |

## Features

### ViewModels
- `ViewModelBase` - Base class with logging, mediator, and busy state
- `PageViewModel` - For page-level ViewModels
- `RegionViewModel` - For nested region ViewModels
- `BaseServices` - DI-friendly service container

### Busy Patterns
- `BusyScope` - Local ViewModel busy state
- `GlobalBusyScope` - App-level busy state via Mediator
- `BusyOverlay` - Control for displaying busy indicator

### UnoCommand Generator
- `[UnoCommand]` - Generates command properties from methods
- Supports `Busy` mode (None, Local, Global)
- Supports `BusyMessage` for display text
- Supports `IncludeCancelCommand` for async cancellation

### Mediator Integration
- `UnoEventCollector` - Collects event handlers from Uno visual tree
- `GlobalBusyEvent` - Event for app-level busy state

## Usage

Add reference to your project:

```xml
<ItemGroup>
  <ProjectReference Include="path/to/UnoFramework.csproj" />
  <ProjectReference Include="path/to/UnoFramework.Contracts.csproj" />
  <ProjectReference Include="path/to/UnoFramework.Generators.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

Register services in App.xaml.cs:

```csharp
using UnoFramework.Configuration;

services.AddShinyMediator();
services.AddShinyServiceRegistry();  // Auto-registers [Service] attributed classes
services.AddUnoFramework();          // Explicit UnoFramework services
```

### Dependency Injection

UnoFramework uses `Shiny.Extensions.DependencyInjection` for automatic service registration.

#### DI Constants

Use `UnoFrameworkService` for consistent service registration:

```csharp
using UnoFramework;

[Service(UnoFrameworkService.Lifetime, TryAdd = UnoFrameworkService.TryAdd)]
public class MyService : IMyService { }
```

| Property | Value | Description |
|----------|-------|-------------|
| `Lifetime` | `Singleton` | Default lifetime for client-side apps |
| `TryAdd` | `true` | Prevents duplicate registrations |

Create a ViewModel:

```csharp
public partial class MyViewModel(BaseServices baseServices) : PageViewModel(baseServices)
{
    [UnoCommand(Busy = BusyMode.Local, BusyMessage = "Loading...")]
    private async Task LoadAsync()
    {
        await Task.Delay(1000);
    }
}
```

## Git Submodule

To use as a git submodule:

```bash
git submodule add <repository-url> framework
```
