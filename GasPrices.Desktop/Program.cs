using System;

using Avalonia;
using Avalonia.Media;


namespace GasPrices.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    // Fix crash in Linux (provide a default font):
    // .With(new FontManagerOptions
    // {
    //     DefaultFamilyName = "avares://GasPrices/Assets/Fonts/GeneralSans_Complete/Fonts/OTF/GeneralSans-Regular.otf#General Sans"
    // });
}
