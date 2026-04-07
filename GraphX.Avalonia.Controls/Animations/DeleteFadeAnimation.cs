using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Animation;
using GraphX.Controls.Models;

namespace GraphX.Controls.Animations
{
    public sealed class DeleteFadeAnimation : IOneWayControlAnimation
    {
        public double Duration { get; set; }

        public DeleteFadeAnimation(double duration = 0.3)
        {
            Duration = duration;
        }

        private void RunAnimation(IGraphControl target, bool removeDataObject)
        {
            var control = target as Control;
            if (control == null)
            {
                OnCompleted(target, removeDataObject);
                return;
            }

            var animation = new Animation
            {
                Duration = TimeSpan.FromSeconds(Duration),
                Children =
                {
                    new KeyFrame
                    {
                        Cue = Cue.Parse("0%"),
                        Setters = { new Setter(Visual.OpacityProperty, 1.0) }
                    },
                    new KeyFrame
                    {
                        Cue = Cue.Parse("100%"),
                        Setters = { new Setter(Visual.OpacityProperty, 0.0) }
                    }
                }
            };

            animation.RunAsync(control).ContinueWith(_ => OnCompleted(target, removeDataObject));
        }

        public void AnimateVertex(VertexControl target, bool removeDataVertex = false)
        {
            RunAnimation(target, removeDataVertex);
        }

        public void AnimateEdge(EdgeControl target, bool removeDataEdge = false)
        {
            RunAnimation(target, removeDataEdge);
        }

        public event RemoveControlEventHandler Completed;

        public void OnCompleted(IGraphControl target, bool removeDataObject)
        {
            Completed?.Invoke(this, new ControlEventArgs(target, removeDataObject));
        }
    }
}
