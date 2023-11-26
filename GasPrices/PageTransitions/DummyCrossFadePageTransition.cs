using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Data;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace GasPrices.PageTransitions;

public class DummyCrossFadePageTransition : IPageTransition
{
        private readonly Animation _fadeOutAnimation;
        private readonly Animation _fadeInAnimation;

        public DummyCrossFadePageTransition(TimeSpan duration)
        {
            _fadeOutAnimation = new Animation
            {
                Children =
                {
                    new KeyFrame()
                    {
                        Setters =
                        {
                            new Setter
                            {
                                Property = Visual.OpacityProperty,
                                Value = 1d
                            }
                        },
                        Cue = new Cue(1d)
                    }
                
                }
            };
            _fadeInAnimation = new Animation
            {
                Children =
                {
                    new KeyFrame()
                    {
                        Setters =
                        {
                            new Setter
                            {
                                Property = Visual.OpacityProperty,
                                Value = 1d
                            }
                        },
                        Cue = new Cue(1d)
                    }
                
                }
            };
            _fadeOutAnimation.Duration = _fadeInAnimation.Duration = duration;
        }

        public TimeSpan Duration
        {
            get => _fadeOutAnimation.Duration;
            set => _fadeOutAnimation.Duration = _fadeInAnimation.Duration = value;
        }

        public Easing FadeInEasing
        {
            get => _fadeInAnimation.Easing;
            set => _fadeInAnimation.Easing = value;
        }

        public Easing FadeOutEasing
        {
            get => _fadeOutAnimation.Easing;
            set => _fadeOutAnimation.Easing = value;
        }

        public async Task Start(Visual? from, Visual? to, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var tasks = new List<Task>();
            using var disposables = new CompositeDisposable(1);
            if (to != null)
            {
                disposables.Add(to.SetValue(Visual.OpacityProperty, 1, BindingPriority.Animation)!);
            }

            if (from != null)
            {
                from.Opacity = 1f;
                tasks.Add(_fadeOutAnimation.RunAsync(from, cancellationToken));
            }

            if (to != null)
            {
                to.Opacity = 1f;
                to.IsVisible = true;
                tasks.Add(_fadeInAnimation.RunAsync(to, cancellationToken));
            }

            await Task.WhenAll(tasks);

            if (from != null && !cancellationToken.IsCancellationRequested)
            {
                from.IsVisible = false;
            }
        }

        Task IPageTransition.Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
        {
            return Start(from, to, cancellationToken);
        }
}