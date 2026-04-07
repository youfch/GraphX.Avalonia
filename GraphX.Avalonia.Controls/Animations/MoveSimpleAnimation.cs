using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Layout;

namespace GraphX.Controls.Animations
{
    public sealed class MoveSimpleAnimation : MoveAnimationBase
    {
        public MoveSimpleAnimation(TimeSpan duration)
        {
            Duration = duration;
        }

        int _maxCount;
        int _counter;

        public override void Cleanup()
        {
        }

        public override async void RunVertexAnimation()
        {
            _maxCount = VertexStorage.Count * 2;
            _counter = 0;

            foreach (var item in VertexStorage)
            {
                var control = item.Key as Control;
                if (control == null) continue;

                var fromX = GraphAreaBase.GetX(control);
                fromX = double.IsNaN(fromX) ? 0.0 : fromX;

                var toX = item.Value.X;
                CreateAndRunAnimation(control, GraphAreaBase.XProperty, fromX, toX, isX: true);

                var fromY = GraphAreaBase.GetY(control);
                fromY = double.IsNaN(fromY) ? 0.0 : fromY;

                var toY = item.Value.Y;
                CreateAndRunAnimation(control, GraphAreaBase.YProperty, fromY, toY, isX: false);
            }
        }

        private void CreateAndRunAnimation(Control control, AvaloniaProperty property, double from, double to, bool isX)
        {
            var animation = new Animation
            {
                Duration = Duration,
                FillMode = FillMode.Forward
            };

            var cueStart = new Cue(0.0);
            var cueEnd = new Cue(1.0);

            var startKeyFrame = new KeyFrame(cueStart);
            startKeyFrame.Setters.Add(new Setter(property, from));

            var endKeyFrame = new KeyFrame(cueEnd);
            endKeyFrame.Setters.Add(new Setter(property, to));

            animation.Children.Add(startKeyFrame);
            animation.Children.Add(endKeyFrame);

            _ = RunAnimationAsync(animation, control, property, to);
        }

        private async System.Threading.Tasks.Task RunAnimationAsync(
            Animation animation, 
            Control control, 
            AvaloniaProperty property, 
            double finalValue)
        {
            try
            {
                await animation.RunAsync(control);
                control.SetValue(property, finalValue);
            }
            finally
            {
                _counter++;
                if (_counter == _maxCount)
                {
                    OnCompleted();
                }
            }
        }
    }
}
