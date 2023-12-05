using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using System;
using System.Threading.Tasks;

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

    private static void IsPastingFromClipboard(object? sender, RoutedEventArgs e)
    {
        Task.Run(async () => await IsPastingFromClipBoardAsync(sender, e));
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.PastingFromClipboard -= IsPastingFromClipboard;
        }

        base.OnDetaching();
    }

    private static async Task IsPastingFromClipBoardAsync(object? sender, RoutedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        var clipboard = TopLevel.GetTopLevel(textBox)!.Clipboard;
        var text = await clipboard!.GetTextAsync() ?? string.Empty;

        if (textBox is { MaxLines: 1, AcceptsReturn: false })
        {
            text = text.Replace(Environment.NewLine, " ") 
                .Replace("\r", " ")
                .Replace("\t", " ");
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " "); 
            }
            if (text.Length > 0 && text[0] == ' ')
            {
                text = text[1..];
            }
            if (text.Length > 0 && text[^1] == ' ')
            {
                text = text[..^1];
            }

            textBox.Text = text;
            e.Handled = true;
        }
    }
}