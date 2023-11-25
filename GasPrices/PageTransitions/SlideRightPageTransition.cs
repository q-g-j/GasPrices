using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;

namespace GasPrices.PageTransitions
{
    public class SlideRightPageTransition : PageSlide
    {
        public override async Task Start(
            Visual? from,
            Visual? to,
            bool forward,
            CancellationToken cancellationToken)
        {
            await base.Start(from, to, false, cancellationToken);
        }
    }
}