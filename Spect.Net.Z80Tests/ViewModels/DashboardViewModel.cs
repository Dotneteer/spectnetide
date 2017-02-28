using Spect.Net.Z80Tests.Mvvm.Attributes;
using Spect.Net.Z80Tests.Mvvm.Navigation;

namespace Spect.Net.Z80Tests.ViewModels
{
    /// <summary>
    /// This class represents the application's dashboard
    /// </summary>
    [ViewResource("\uE10F")]
    [ViewTitle("Dashboard")]
    [IsSingleton]
    [ExcludeFromNavigationHistory]
    public class DashboardViewModel : NavigableViewModelBase
    {
    }
}