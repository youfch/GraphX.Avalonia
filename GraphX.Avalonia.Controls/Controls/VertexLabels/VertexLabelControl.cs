using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using GraphX.Common.Enums;
using GraphX.Common.Exceptions;

namespace GraphX.Controls
{
    public class VertexLabelControl : ContentControl, IVertexLabelControl
    {
        internal Rect LastKnownRectSize;

        public static readonly StyledProperty<double> AngleProperty =
            AvaloniaProperty.Register<VertexLabelControl, double>(nameof(Angle), 0.0, AngleChanged);

        private static void AngleChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var ctrl = d as IControl;
            if (ctrl == null)
                return;
            var tg = ctrl.RenderTransform as TransformGroup;
            if (tg == null)
                ctrl.RenderTransform = new RotateTransform { Angle = (double)e.NewValue!, CenterX = 0.5, CenterY = 0.5 };
            else
            {
                var rt = tg.Children.FirstOrDefault(a => a is RotateTransform);
                if (rt == null)
                    tg.Children.Add(new RotateTransform { Angle = (double)e.NewValue!, CenterX = 0.5, CenterY = 0.5 });
                else (rt as RotateTransform)!.Angle = (double)e.NewValue!;
            }
        }

        /// <summary>
        /// Gets or sets label drawing angle in degrees
        /// </summary>
        public double Angle
        {
            get => GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly StyledProperty<Point> LabelPositionProperty =
            AvaloniaProperty.Register<VertexLabelControl, Point>(nameof(LabelPosition), new Point());

        /// <summary>
        /// Gets or sets label position if LabelPositionMode is set to Coordinates
        /// Position is always measured from top left VERTEX corner.
        /// </summary>
        public Point LabelPosition
        {
            get => GetValue(LabelPositionProperty);
            set => SetValue(LabelPositionProperty, value);
        }

        public static readonly StyledProperty<VertexLabelPositionMode> LabelPositionModeProperty =
            AvaloniaProperty.Register<VertexLabelControl, VertexLabelPositionMode>(nameof(LabelPositionMode), VertexLabelPositionMode.Sides);

        /// <summary>
        /// Gets or set label positioning mode
        /// </summary>
        public VertexLabelPositionMode LabelPositionMode
        {
            get => GetValue(LabelPositionModeProperty);
            set => SetValue(LabelPositionModeProperty, value);
        }

        public static readonly StyledProperty<VertexLabelPositionSide> LabelPositionSideProperty =
            AvaloniaProperty.Register<VertexLabelControl, VertexLabelPositionSide>(nameof(LabelPositionSide), VertexLabelPositionSide.BottomRight);

        /// <summary>
        /// Gets or sets label position side if LabelPositionMode is set to Sides
        /// </summary>
        public VertexLabelPositionSide LabelPositionSide
        {
            get => GetValue(LabelPositionSideProperty);
            set => SetValue(LabelPositionSideProperty, value);
        }

        public VertexLabelControl()
        {
            LayoutUpdated += VertexLabelControl_LayoutUpdated;
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
        }

        protected virtual VertexControl? GetVertexControl(Control? parent)
        {
            while (parent != null)
            {
                var control = parent as VertexControl;
                if (control != null) return control;
                parent = control?.GetVisualParent();
            }
            return null;
        }

        public virtual void UpdatePosition()
        {
            if (double.IsNaN(DesiredSize.Width) || DesiredSize.Width == 0) return;

            var vc = GetVertexControl(GetParent());
            if (vc == null) return;

            if (LabelPositionMode == VertexLabelPositionMode.Sides)
            {
                Point pt;
                switch (LabelPositionSide)
                {
                    case VertexLabelPositionSide.TopRight:
                        pt = new Point(vc.DesiredSize.Width, -DesiredSize.Height);
                        break;
                    case VertexLabelPositionSide.BottomRight:
                        pt = new Point(vc.DesiredSize.Width, vc.DesiredSize.Height);
                        break;
                    case VertexLabelPositionSide.TopLeft:
                        pt = new Point(-DesiredSize.Width, -DesiredSize.Height);
                        break;
                    case VertexLabelPositionSide.BottomLeft:
                        pt = new Point(-DesiredSize.Width, vc.DesiredSize.Height);
                        break;
                    case VertexLabelPositionSide.Top:
                        pt = new Point(vc.DesiredSize.Width * .5 - DesiredSize.Width * .5, -DesiredSize.Height);
                        break;
                    case VertexLabelPositionSide.Bottom:
                        pt = new Point(vc.DesiredSize.Width * .5 - DesiredSize.Width * .5, vc.DesiredSize.Height);
                        break;
                    case VertexLabelPositionSide.Left:
                        pt = new Point(-DesiredSize.Width, vc.DesiredSize.Height * .5 - DesiredSize.Height * .5);
                        break;
                    case VertexLabelPositionSide.Right:
                        pt = new Point(vc.DesiredSize.Width, vc.DesiredSize.Height * .5 - DesiredSize.Height * .5);
                        break;
                    default:
                        throw new GX_InvalidDataException("UpdatePosition() -> Unknown vertex label side!");
                }
                LastKnownRectSize = new Rect(pt, DesiredSize);
            }
            else LastKnownRectSize = new Rect(LabelPosition, DesiredSize);

            Arrange(LastKnownRectSize);
        }

        public void Hide()
        {
            IsVisible = false;
        }

        public void Show()
        {
            IsVisible = true;
        }

        void VertexLabelControl_LayoutUpdated(object? sender, EventArgs e)
        {
            var vc = GetVertexControl(GetParent());
            if (vc == null || !vc.ShowLabel) return;
            UpdatePosition();
        }

        protected virtual Control? GetParent()
        {
            return this.GetVisualParent();
        }
    }
}
