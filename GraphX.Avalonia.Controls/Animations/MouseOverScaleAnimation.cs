using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Animation;

namespace GraphX.Controls.Animations
{
    public sealed class MouseOverScaleAnimation : IBidirectionalControlAnimation
    {
        /// <summary>
        /// Scale to this value. Default size is 1. For ex. 2 will double the size of the object.
        /// </summary>
        public double ScaleTo { get; set; }
        /// <summary>
        /// Scale from the center of the object or from the left top corner
        /// </summary>
        public bool CenterScale { get; set; }

        /// <summary>
        /// Animation duration in milliseconds.
        /// </summary>
        public double Duration { get; set; }

        public MouseOverScaleAnimation(double duration = .3, double scaleto = 1.2, bool centerscale = true)
        {
            Duration = duration;
            ScaleTo = scaleto;
            CenterScale = centerscale;
        }

        public void AnimateVertexForward(VertexControl target)
        {
            var transform = CustomHelper.GetScaleTransform(target);
            if (transform == null)
            {
                transform = new ScaleTransform();
                target.RenderTransform = transform;
                target.RenderTransformOrigin = CenterScale 
                    ? new RelativePoint(.5, .5, RelativeUnit.Relative) 
                    : new RelativePoint(0, 0, RelativeUnit.Relative);
            }

            transform.Transitions = new Transitions
            {
                new DoubleTransition(ScaleTransform.ScaleXProperty, TimeSpan.FromSeconds(Duration)),
                new DoubleTransition(ScaleTransform.ScaleYProperty, TimeSpan.FromSeconds(Duration))
            };
            transform.ScaleX = ScaleTo;
            transform.ScaleY = ScaleTo;
        }

        public void AnimateVertexBackward(VertexControl target)
        {
            var transform = CustomHelper.GetScaleTransform(target);
            if (transform == null)
            {
                transform = new ScaleTransform();
                target.RenderTransform = transform;
                target.RenderTransformOrigin = CenterScale 
                    ? new RelativePoint(.5, .5, RelativeUnit.Relative) 
                    : new RelativePoint(0, 0, RelativeUnit.Relative);
                return;
            }

            if (transform.ScaleX <= 1 || transform.ScaleY <= 1) return;

            transform.Transitions = new Transitions
            {
                new DoubleTransition(ScaleTransform.ScaleXProperty, TimeSpan.FromSeconds(Duration)),
                new DoubleTransition(ScaleTransform.ScaleYProperty, TimeSpan.FromSeconds(Duration))
            };
            transform.ScaleX = 1;
            transform.ScaleY = 1;
        }

        public void AnimateEdgeForward(EdgeControl target)
        {
        }

        public void AnimateEdgeBackward(EdgeControl target)
        {
        }
    }
}
