using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;

namespace GasPrices.PageTransitions;

public class SlideRightPageTransition : PageSlide, ICustomPageTransition
{
    public override Task Start(
        Visual? from,
        Visual? to,
        bool forward,
        CancellationToken cancellationToken)
    {
        to!.Opacity = 1;
        return base.Start(from, to, false, cancellationToken);
    }
}