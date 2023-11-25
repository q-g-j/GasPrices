// Decompiled with JetBrains decompiler
// Type: Avalonia.Animation.PageSlide
// Assembly: Avalonia.Base, Version=11.0.2.0, Culture=neutral, PublicKeyToken=c8d484a7012f9a8b
// MVID: 40819391-5037-4DF0-8A07-A87A6D200291
// Assembly location: C:\Users\Jann\.nuget\packages\avalonia\11.0.2\ref\net6.0\Avalonia.Base.dll
// XML documentation location: C:\Users\Jann\.nuget\packages\avalonia\11.0.2\ref\net6.0\Avalonia.Base.xml

#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;

namespace GasPrices.PageTransitions
{
    /// <summary>
    /// Transitions between two pages by sliding them horizontally or vertically.
    /// </summary>
    public class SlideRightPageTransition : PageSlide
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GasPrices.PageTransitions.PageSlide" /> class.
        /// </summary>
        public SlideRightPageTransition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GasPrices.PageTransitions.PageSlide" /> class.
        /// </summary>
        /// <param name="duration">The duration of the animation.</param>
        /// <param name="orientation">The axis on which the animation should occur</param>
        public SlideRightPageTransition(TimeSpan duration,
            PageSlide.SlideAxis orientation = PageSlide.SlideAxis.Horizontal)
        {
            this.Duration = duration;
            this.Orientation = orientation;
        }

        /// <inheritdoc />
        public override async Task Start(
            Visual? from,
            Visual? to,
            bool forward,
            CancellationToken cancellationToken)
        {
            forward = false;
            if (cancellationToken.IsCancellationRequested)
                return;
            List<Task> taskList = new List<Task>();
            Visual visualParent = GetVisualParent(from, to);
            double num = this.Orientation == PageSlide.SlideAxis.Horizontal
                ? visualParent.Bounds.Width
                : visualParent.Bounds.Height;
            StyledProperty<double> styledProperty = this.Orientation == PageSlide.SlideAxis.Horizontal
                ? TranslateTransform.XProperty
                : TranslateTransform.YProperty;
            if (from != null)
            {
                Avalonia.Animation.Animation animation1 = new Avalonia.Animation.Animation();
                animation1.Easing = SlideOutEasing;
                animation1.Children.Add(new KeyFrame()
                {
                    Setters =
                    {
                        (IAnimationSetter)new Setter()
                        {
                            Property = (AvaloniaProperty)styledProperty,
                            Value = (object)0.0
                        }
                    },
                    Cue = new Cue(0.0)
                });
                animation1.Children.Add(new KeyFrame()
                {
                    Setters =
                    {
                        (IAnimationSetter)new Setter()
                        {
                            Property = (AvaloniaProperty)styledProperty,
                            Value = (object)(forward ? -num : num)
                        }
                    },
                    Cue = new Cue(1.0)
                });
                animation1.Duration = this.Duration;
                Avalonia.Animation.Animation animation2 = animation1;
                taskList.Add(animation2.RunAsync((Animatable)from, cancellationToken));
            }

            if (to != null)
            {
                to.IsVisible = true;
                Avalonia.Animation.Animation animation3 = new Avalonia.Animation.Animation();
                animation3.Easing = this.SlideInEasing;
                animation3.Children.Add(new KeyFrame()
                {
                    Setters =
                    {
                        (IAnimationSetter)new Setter()
                        {
                            Property = (AvaloniaProperty)styledProperty,
                            Value = (object)(forward ? num : -num)
                        }
                    },
                    Cue = new Cue(0.0)
                });
                animation3.Children.Add(new KeyFrame()
                {
                    Setters =
                    {
                        (IAnimationSetter)new Setter()
                        {
                            Property = (AvaloniaProperty)styledProperty,
                            Value = (object)0.0
                        }
                    },
                    Cue = new Cue(1.0)
                });
                animation3.Duration = this.Duration;
                Avalonia.Animation.Animation animation4 = animation3;
                taskList.Add(animation4.RunAsync((Animatable)to, cancellationToken));
            }

            await Task.WhenAll((IEnumerable<Task>)taskList);
            if (from == null || cancellationToken.IsCancellationRequested)
                return;
            from.IsVisible = false;
        }
    }
}