using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using System;
using Avalonia;

namespace GasPrices.Behaviours;

public class TextBoxMaxLinesPasteBehaviour : Behavior<TextBox>
{
    protected override void OnAttached()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.PastingFromClipboard += IsPastingFromClipboard;
        }

        base.OnAttached();
    }

    private async void IsPastingFromClipboard(object? sender, RoutedEventArgs e)
    {
        IClipboard clipboard = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow!.Clipboard!;
        var t = await clipboard.GetTextAsync() ?? string.Empty;

        if (AssociatedObject is not null && AssociatedObject.MaxLines == 1 && !AssociatedObject.AcceptsReturn)
        {
            AssociatedObject.Text = t.Replace(Environment.NewLine, " ");
            e.Handled = true;
            return;
        }

        e.Handled = false;
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.PastingFromClipboard -= IsPastingFromClipboard;
        }

        base.OnDetaching();
    }
}