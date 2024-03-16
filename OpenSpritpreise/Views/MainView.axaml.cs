using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace OpenSpritpreise.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs onLoadedEvent)
    {
        base.OnLoaded(onLoadedEvent);

        var topLevel = TopLevel.GetTopLevel(this);
        topLevel!.BackRequested += (_, backRequestedEvent) =>
        {
            var app = App.GetCurrent();
            if (!app.IsBackPressedSubscribed()) return;
            
            backRequestedEvent.Handled = true;
            app.OnBackPressed();
        };
    }
}
