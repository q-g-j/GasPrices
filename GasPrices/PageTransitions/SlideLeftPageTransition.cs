using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;

namespace GasPrices.PageTransitions;

public class SlideLeftPageTransition(
    TimeSpan duration,
    PageSlide.SlideAxis orientation = PageSlide.SlideAxis.Horizontal)
    : PageSlide(duration,
        orientation)
{
    public override Task Start(
        Visual? from,
        Visual? to,
        bool forward,
        CancellationToken cancellationToken)
    {
        to!.Opacity = 1;
        return base.Start(from, to, true, cancellationToken);
    }
}