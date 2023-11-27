using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;

namespace GasPrices.PageTransitions;

public class SlideLeftPageTransition : PageSlide
{
    public SlideLeftPageTransition()
    {
    }

    public SlideLeftPageTransition(TimeSpan duration, SlideAxis orientation = SlideAxis.Horizontal) : base(duration, orientation)
    {
    }

    public override async Task Start(
        Visual? from,
        Visual? to,
        bool forward,
        CancellationToken cancellationToken)
    {
        to!.Opacity = 1;
        await base.Start(from, to, true, cancellationToken);
    }
}
