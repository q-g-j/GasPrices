using System;
using Avalonia.Animation;

namespace OpenSpritpreise.PageTransitions;

public interface ICustomPageTransition : IPageTransition
{
    TimeSpan Duration { get; set; }
}