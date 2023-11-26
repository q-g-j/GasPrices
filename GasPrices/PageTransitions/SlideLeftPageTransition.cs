using System;
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
}
