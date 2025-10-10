using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LazyVoom.ServiceProvider;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] int count;

    [RelayCommand]
    private void Increment()
    {
        Count++;
    }

    [RelayCommand]
    private void Decrement()
    {
        Count--;
    }
}
