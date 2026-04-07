using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Animation;
using GraphX.Common.Exceptions;
using GraphX.Controls.Models;

namespace GraphX.Controls.Animations
{
    public sealed class MoveFadeAnimation : MoveAnimationBase
    {
        public MoveFadeAnimation(TimeSpan duration)
        {
            Duration = duration;
        }
        
        int _vMaxCount;
        int _vCounter;
        int _eMaxCount;
        int _eCounter;

        private bool IsDefaultCoordinates()
        {
            var ptZero = new Point();
            return VertexStorage.Keys.All(item => item.GetPosition() == ptZero);
        }

        public override void RunVertexAnimation()
        {
            _vMaxCount = VertexStorage.Count;
            _vCounter = 0;
            var defaultValues = IsDefaultCoordinates();

            foreach (var item in VertexStorage)
            {
                if (item.Key is EdgeControl) throw new GX_InvalidDataException("AnimateVertex() -> Got edge control instead vertex control!");

                var control = item.Key as Control;
                if (defaultValues)
                {
                    GraphAreaBase.SetX(control, GraphAreaBase.GetFinalX(control));
                    GraphAreaBase.SetY(control, GraphAreaBase.GetFinalY(control));
                    FadeAnimation(control, 0, 1, (o, e) =>
                    {
                        _vCounter++;
                        if (_vCounter == _vMaxCount)
                            OnCompleted();
                    });
                }
                else
                {
                    FadeAnimation(control, 1, 0, async (o, e) =>
                    {
                        if (!VertexStorage.ContainsKey(item.Key))
                            return;
                        
                        GraphAreaBase.SetX(control, GraphAreaBase.GetFinalX(control));
                        GraphAreaBase.SetY(control, GraphAreaBase.GetFinalY(control));

                        await Task.Delay(10);
                        
                        FadeAnimation(control, 0, 1, (o2, e2) =>
                        {
                            _vCounter++;
                            if (_vCounter == _vMaxCount)
                                OnCompleted();
                        });
                    });
                }
            }
        }

        public override void RunEdgeAnimation()
        {
            _eMaxCount = EdgeStorage.Count;
            _eCounter = 0;

            foreach (var item in EdgeStorage)
            {
                if (item is VertexControl) throw new GX_InvalidDataException("AnimateEdge() -> Got vertex control instead edge control!");
                
                var control = item as Control;
                FadeAnimation(control, 1, 0, async (o, e) =>
                {
                    await Task.Delay(10);
                    FadeAnimation(control, 0, 1, (o2, e2) =>
                    {
                        _eCounter++;
                        if (_eCounter == _eMaxCount)
                            OnCompleted();
                    });
                });
            }
        }

        private void FadeAnimation(Control control, double from, double to, EventHandler callback)
        {
            var animation = new Animation
            {
                Duration = Duration,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = Cue.Parse("0%"),
                        Setters = { new Setter(Visual.OpacityProperty, from) }
                    },
                    new KeyFrame
                    {
                        Cue = Cue.Parse("100%"),
                        Setters = { new Setter(Visual.OpacityProperty, to) }
                    }
                }
            };

            animation.RunAsync(control).ContinueWith(_ => callback?.Invoke(this, EventArgs.Empty));
        }

        public override void Cleanup()
        {
        }
    }
}
