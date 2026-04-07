using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using GraphX.Controls.Models;

namespace GraphX.Controls.Animations
{
    public sealed class DeleteShrinkAnimation : IOneWayControlAnimation
    {
        public double Duration { get; set; }
        public bool Centered { get; set; }

        public DeleteShrinkAnimation(double duration = .3, bool centered = true)
        {
            Duration = duration;
            Centered = centered;
        }

        public async void AnimateVertex(VertexControl target, bool removeDataVertex = false)
        {
            var transform = target.RenderTransform as ScaleTransform;
            if (transform == null)
            {
                transform = new ScaleTransform();
                target.RenderTransform = transform;
                target.RenderTransformOrigin = new RelativePoint(Centered ? 0.5 : 0, Centered ? 0.5 : 0, RelativeUnit.Relative);
            }

            transform.Transitions = new Transitions
            {
                new DoubleTransition(ScaleTransform.ScaleXProperty, TimeSpan.FromSeconds(Duration)),
                new DoubleTransition(ScaleTransform.ScaleYProperty, TimeSpan.FromSeconds(Duration))
            };

            transform.ScaleX = 0;
            transform.ScaleY = 0;

            await Task.Delay(TimeSpan.FromSeconds(Duration));
            OnCompleted(target, removeDataVertex);
        }

        public void AnimateEdge(EdgeControl target, bool removeDataEdge = false)
        {
            OnCompleted(target, removeDataEdge);
        }

        public event RemoveControlEventHandler Completed;

        private void OnCompleted(IGraphControl target, bool removeDataObject)
        {
            Completed?.Invoke(this, new RemoveControlEventArgs(target, removeDataObject));
        }
    }
}
