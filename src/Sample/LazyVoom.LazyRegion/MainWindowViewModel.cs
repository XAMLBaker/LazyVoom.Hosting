using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LazyRegion.Core;

namespace LazyVoom.LazyRegion;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ILazyRegionManager _lazyRegionManager;
    public MainWindowViewModel(ILazyRegionManager lazyRegionManager)
    {
        this._lazyRegionManager = lazyRegionManager;
    }

    [RelayCommand]
    private void Move(string param)
    {
        this._lazyRegionManager.NavigateAsync ("Root", param);
    }
}
