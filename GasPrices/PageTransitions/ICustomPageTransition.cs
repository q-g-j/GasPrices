using System;
using Avalonia.Animation;

namespace GasPrices.PageTransitions;

public interface ICustomPageTransition : IPageTransition
{
    TimeSpan Duration { get; set; }
}