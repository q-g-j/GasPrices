using System;
using Avalonia.Animation;

namespace GasPrices.PageTransitions;

public class CustomCompositePageTransition<TPageTransition1, TPageTransition2>
    : CompositePageTransition, ICustomPageTransition
    where TPageTransition1 : ICustomPageTransition, new()
    where TPageTransition2 : ICustomPageTransition, new()
{
    public CustomCompositePageTransition()
    {
        Duration = TimeSpan.FromMilliseconds(400);

        PageTransitions.Add(new TPageTransition1
        {
            Duration = Duration
        });
        PageTransitions.Add(new TPageTransition2
        {
            Duration = Duration
        });
    }

    public TimeSpan Duration { get; set; }
}