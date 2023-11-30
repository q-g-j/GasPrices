using System;
using Avalonia.Animation;

namespace GasPrices.PageTransitions;

public class CustomCrossFadePageTransition(TimeSpan duration) : CrossFade(duration), ICustomPageTransition
{
    public CustomCrossFadePageTransition() : this(TimeSpan.FromMilliseconds(400))
    {
        Duration = TimeSpan.FromMilliseconds(400);
    }
}