using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace GraphX.Controls
{
    /// <summary>
    /// Edge pointer control for edge endpoints customization
    /// Represents ContentControl that can host different content, e.g. Image or Path
    /// </summary>
    public class DefaultEdgePointer : ContentControl, IEdgePointer
    {
        internal Rect LastKnownRectSize;

        public static readonly StyledProperty<Point> OffsetProperty =
            AvaloniaProperty.Register<DefaultEdgePointer, Point>(nameof(Offset));

        /// <summary>
        /// Gets or sets offset for the image position
        /// </summary>
        public Point Offset
        {
            get => GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        public static readonly StyledProperty<bool> NeedRotationProperty =
            AvaloniaProperty.Register<DefaultEdgePointer, bool>(nameof(NeedRotation), true);

        /// <inheritdoc />
        public bool NeedRotation
        {
            get => GetValue(NeedRotationProperty);
            set => SetValue(NeedRotationProperty, value);
        }

        /// <inheritdoc />
        public Point GetPosition()
        {
            return LastKnownRectSize.IsEmpty ? new Point() : LastKnownRectSize.Center;
        }

        /// <inheritdoc />
        public void Show()
        {
            if (IsSuppressed)
                return;
            IsVisible = true;
        }

        /// <inheritdoc />
        public void Hide()
        {
            IsVisible = false;
        }

        /// <summary>
        /// Suppresses the pointer from view, overriding the Visibility value until unsuppressed.
        /// </summary>
        public void Suppress()
        {
            IsSuppressed = true;
            IsVisible = false;
        }

        /// <summary>
        /// Removes the suppression constraint, returning control to normal visibility state.
        /// </summary>
        public void UnSuppress()
        {
            IsSuppressed = false;
        }

        public static readonly StyledProperty<bool> IsSuppressedProperty =
            AvaloniaProperty.Register<DefaultEdgePointer, bool>(nameof(IsSuppressed), false);

        /// <summary>
        /// Gets a value indicating whether the pointer is suppressed. A suppressed pointer won't be displayed
        /// </summary>
        public bool IsSuppressed
        {
            get => GetValue(IsSuppressedProperty);
            private set => SetValue(IsSuppressedProperty, value);
        }

        private EdgeControl _edgeControl;
        protected EdgeControl EdgeControl => _edgeControl ?? (_edgeControl = GetEdgeControl(GetParent()));

        public DefaultEdgePointer()
        {
            RenderTransformOrigin = new RelativePoint(.5, .5, RelativeUnit.Relative);
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            LayoutUpdated += EdgePointer_LayoutUpdated;
        }

        /// <summary>
        /// Update edge pointer position and angle
        /// </summary>
        public virtual Point Update(Point? position, Vector direction, double angle = 0d)
        {
            if (DesiredSize.Width == 0 || DesiredSize.Height == 0 || !position.HasValue) return new Point();
            var vecMove = new Vector(direction.X * DesiredSize.Width * .5, direction.Y * DesiredSize.Height * .5);
            position = new Point(position.Value.X - vecMove.X, position.Value.Y - vecMove.Y);
            if (!double.IsNaN(DesiredSize.Width) && DesiredSize.Width != 0 && !double.IsNaN(position.Value.X))
            {
                LastKnownRectSize = new Rect(new Point(position.Value.X - DesiredSize.Width * .5, position.Value.Y - DesiredSize.Height * .5), DesiredSize);
                Arrange(LastKnownRectSize);
            }

            try
            {
                if (NeedRotation)
                    RenderTransform = new RotateTransform { Angle = double.IsNaN(angle) ? 0 : angle };
            }
            catch (Exception)
            {
            }

            return new Point(direction.X * ActualWidth, direction.Y * ActualHeight);
        }

        public void SetManualPosition(Point position)
        {
            LastKnownRectSize = new Rect(new Point(position.X - DesiredSize.Width * .5, position.Y - DesiredSize.Height * .5), DesiredSize);
            Arrange(LastKnownRectSize);
        }

        public void Dispose()
        {
            _edgeControl = null;
        }

        void EdgePointer_LayoutUpdated(object? sender, EventArgs e)
        {
            if (LastKnownRectSize != Rect.Empty && !double.IsNaN(LastKnownRectSize.Width) && LastKnownRectSize.Width != 0
                && EdgeControl != null)
                Arrange(LastKnownRectSize);
        }

        private IVisual GetParent()
        {
            return this.GetVisualParent();
        }

        private static EdgeControl GetEdgeControl(IVisual parent)
        {
            while (parent != null)
            {
                if (parent is EdgeControl control) return control;
                parent = parent.GetVisualParent();
            }
            return null;
        }
    }
}
