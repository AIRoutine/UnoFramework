using Microsoft.UI.Xaml.Navigation;
using UnoFramework.ViewModels;

namespace UnoFramework.Pages;

/// <summary>
/// Base page class that automatically triggers PageViewModel lifecycle methods.
/// Calls OnNavigatedToAsync and OnNavigatedFromAsync on PageViewModel instances.
/// </summary>
public class BasePage : Page
{
    /// <summary>
    /// Called when the page is navigated to. Triggers PageViewModel.OnNavigatedToAsync if DataContext is a PageViewModel.
    /// </summary>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (DataContext is PageViewModel viewModel)
        {
            await viewModel.OnNavigatedToAsync(e);
        }
    }

    /// <summary>
    /// Called when the page is navigated away from. Triggers PageViewModel.OnNavigatedFromAsync if DataContext is a PageViewModel.
    /// </summary>
    protected override async void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);

        if (DataContext is PageViewModel viewModel)
        {
            await viewModel.OnNavigatedFromAsync(e);
        }
    }
}
