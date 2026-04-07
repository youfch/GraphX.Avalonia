using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

namespace GraphX.Controls
{
    /// <summary>
    /// Zoom control for zooming and panning content.
    /// Provides mouse wheel zoom and drag-to-pan functionality.
    /// </summary>
    [TemplatePart(name: PART_PRESENTER, type: typeof(ZoomContentPresenter))]
    public class ZoomControl : ContentControl, IZoomControl, INotifyPropertyChanged
    {
        private const string PART_PRESENTER = "PART_Presenter";

        #region Private Fields

        private ZoomContentPresenter? _presenter;
        private ScaleTransform? _scaleTransform;
        private TranslateTransform? _translateTransform;
        private TransformGroup? _transformGroup;
        private bool _isDragging;
        private Point _lastMousePosition;
        private bool _isZooming;

        #endregion

        #region Styled Properties

        /// <summary>
        /// Gets or sets the current zoom level.
        /// </summary>
        public static readonly StyledProperty<double> ZoomProperty =
            AvaloniaProperty.Register<ZoomControl, double>(nameof(Zoom), 1.0);

        /// <summary>
        /// Gets or sets the X translation.
        /// </summary>
        public static readonly StyledProperty<double> TranslateXProperty =
            AvaloniaProperty.Register<ZoomControl, double>(nameof(TranslateX), 0.0);

        /// <summary>
        /// Gets or sets the Y translation.
        /// </summary>
        public static readonly StyledProperty<double> TranslateYProperty =
            AvaloniaProperty.Register<ZoomControl, double>(nameof(TranslateY), 0.0);

        /// <summary>
        /// Gets or sets the minimum zoom level.
        /// </summary>
        public static readonly StyledProperty<double> MinZoomProperty =
            AvaloniaProperty.Register<ZoomControl, double>(nameof(MinZoom), 0.01);

        /// <summary>
        /// Gets or sets the maximum zoom level.
        /// </summary>
        public static readonly StyledProperty<double> MaxZoomProperty =
            AvaloniaProperty.Register<ZoomControl, double>(nameof(MaxZoom), 100.0);

        /// <summary>
        /// Gets or sets the zoom step for mouse wheel.
        /// </summary>
        public static readonly StyledProperty<double> ZoomStepProperty =
            AvaloniaProperty.Register<ZoomControl, double>(nameof(ZoomStep), 0.1);

        /// <summary>
        /// Gets or sets whether panning is enabled.
        /// </summary>
        public static readonly StyledProperty<bool> IsPanEnabledProperty =
            AvaloniaProperty.Register<ZoomControl, bool>(nameof(IsPanEnabled), true);

        /// <summary>
        /// Gets or sets whether zoom is enabled.
        /// </summary>
        public static readonly StyledProperty<bool> IsZoomEnabledProperty =
            AvaloniaProperty.Register<ZoomControl, bool>(nameof(IsZoomEnabled), true);

        /// <summary>
        /// Gets or sets the mouse wheel zooming mode.
        /// </summary>
        public static readonly StyledProperty<MouseWheelZoomingMode> MouseWheelZoomingModeProperty =
            AvaloniaProperty.Register<ZoomControl, MouseWheelZoomingMode>(nameof(MouseWheelZoomingMode), 
                MouseWheelZoomingMode.ToMousePosition);

        /// <summary>
        /// Gets or sets the animation length.
        /// </summary>
        public static readonly StyledProperty<TimeSpan> AnimationLengthProperty =
            AvaloniaProperty.Register<ZoomControl, TimeSpan>(nameof(AnimationLength), TimeSpan.FromMilliseconds(200));

        /// <summary>
        /// Gets or sets the current mode.
        /// </summary>
        public static readonly StyledProperty<ZoomControlModes> ModeProperty =
            AvaloniaProperty.Register<ZoomControl, ZoomControlModes>(nameof(Mode), ZoomControlModes.Original);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current zoom level.
        /// </summary>
        public double Zoom
        {
            get => GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, Math.Clamp(value, MinZoom, MaxZoom));
        }

        /// <summary>
        /// Gets or sets the X translation.
        /// </summary>
        public double TranslateX
        {
            get => GetValue(TranslateXProperty);
            set => SetValue(TranslateXProperty, value);
        }

        /// <summary>
        /// Gets or sets the Y translation.
        /// </summary>
        public double TranslateY
        {
            get => GetValue(TranslateYProperty);
            set => SetValue(TranslateYProperty, value);
        }

        /// <summary>
        /// Gets or sets the minimum zoom level.
        /// </summary>
        public double MinZoom
        {
            get => GetValue(MinZoomProperty);
            set => SetValue(MinZoomProperty, value);
        }

        /// <summary>
        /// Gets or sets the maximum zoom level.
        /// </summary>
        public double MaxZoom
        {
            get => GetValue(MaxZoomProperty);
            set => SetValue(MaxZoomProperty, value);
        }

        /// <summary>
        /// Gets or sets the zoom step for mouse wheel.
        /// </summary>
        public double ZoomStep
        {
            get => GetValue(ZoomStepProperty);
            set => SetValue(ZoomStepProperty, value);
        }

        /// <summary>
        /// Gets or sets whether panning is enabled.
        /// </summary>
        public bool IsPanEnabled
        {
            get => GetValue(IsPanEnabledProperty);
            set => SetValue(IsPanEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets whether zoom is enabled.
        /// </summary>
        public bool IsZoomEnabled
        {
            get => GetValue(IsZoomEnabledProperty);
            set => SetValue(IsZoomEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets the mouse wheel zooming mode.
        /// </summary>
        public MouseWheelZoomingMode MouseWheelZoomingMode
        {
            get => GetValue(MouseWheelZoomingModeProperty);
            set => SetValue(MouseWheelZoomingModeProperty, value);
        }

        /// <summary>
        /// Gets or sets the animation length.
        /// </summary>
        public TimeSpan AnimationLength
        {
            get => GetValue(AnimationLengthProperty);
            set => SetValue(AnimationLengthProperty, value);
        }

        /// <summary>
        /// Gets or sets the current mode.
        /// </summary>
        public ZoomControlModes Mode
        {
            get => GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        /// <summary>
        /// Gets the visual presenter.
        /// </summary>
        public Visual? PresenterVisual => _presenter;

        #endregion

        #region IZoomControl Members

        double IZoomControl.Width => Bounds.Width;
        double IZoomControl.Height => Bounds.Height;

        #endregion

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Constructor

        public ZoomControl()
        {
            AddHandler(PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);
            AddHandler(PointerPressedEvent, OnPointerPressedHandler, RoutingStrategies.Tunnel);
            AddHandler(PointerReleasedEvent, OnPointerReleasedHandler, RoutingStrategies.Tunnel);
            AddHandler(PointerMovedEvent, OnPointerMovedHandler, RoutingStrategies.Tunnel);
        }

        #endregion

        #region Template

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _presenter = e.NameScope.Find<ZoomContentPresenter>(PART_PRESENTER);

            if (_presenter != null)
            {
                InitializeTransforms();
                _presenter.ContentSizeChanged += OnContentSizeChanged;
            }
        }

        private void InitializeTransforms()
        {
            if (_presenter == null)
                return;

            _scaleTransform = new ScaleTransform(Zoom, Zoom);
            _translateTransform = new TranslateTransform(TranslateX, TranslateY);

            _transformGroup = new TransformGroup();
            _transformGroup.Children.Add(_scaleTransform);
            _transformGroup.Children.Add(_translateTransform);

            _presenter.RenderTransform = _transformGroup;
            _presenter.RenderTransformOrigin = RelativePoint.TopLeft;
        }

        private void OnContentSizeChanged(Size newSize)
        {
            UpdateViewport();
        }

        #endregion

        #region Property Changed Callbacks

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ZoomProperty)
            {
                OnZoomChanged(change.GetOldValue<double>(), change.GetNewValue<double>());
            }
            else if (change.Property == TranslateXProperty)
            {
                OnTranslateXChanged(change.GetNewValue<double>());
            }
            else if (change.Property == TranslateYProperty)
            {
                OnTranslateYChanged(change.GetNewValue<double>());
            }
        }

        private void OnZoomChanged(double oldZoom, double newZoom)
        {
            if (_scaleTransform == null)
                return;

            _scaleTransform.ScaleX = newZoom;
            _scaleTransform.ScaleY = newZoom;

            if (!_isZooming)
            {
                var delta = newZoom / oldZoom;
                TranslateX *= delta;
                TranslateY *= delta;
                Mode = ZoomControlModes.Custom;
            }

            UpdateViewport();
            OnPropertyChanged(nameof(Zoom));
        }

        private void OnTranslateXChanged(double value)
        {
            if (_translateTransform != null)
            {
                _translateTransform.X = value;
                UpdateViewport();
            }
        }

        private void OnTranslateYChanged(double value)
        {
            if (_translateTransform != null)
            {
                _translateTransform.Y = value;
                UpdateViewport();
            }
        }

        #endregion

        #region Input Handling

        private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (!IsZoomEnabled || _presenter == null)
                return;

            e.Handled = true;

            var mousePos = e.GetCurrentPoint(this).Position;
            var delta = e.Delta.Y > 0 ? ZoomStep : -ZoomStep;
            var newZoom = Math.Clamp(Zoom * (1 + delta), MinZoom, MaxZoom);

            if (newZoom == Zoom)
                return;

            if (MouseWheelZoomingMode == MouseWheelZoomingMode.ToMousePosition)
            {
                // Zoom to mouse position
                var zoomPoint = mousePos;
                var oldTranslateX = TranslateX;
                var oldTranslateY = TranslateY;

                _isZooming = true;
                Zoom = newZoom;

                var scale = newZoom / (Zoom / (1 + delta));
                TranslateX = zoomPoint.X - (zoomPoint.X - oldTranslateX) * scale;
                TranslateY = zoomPoint.Y - (zoomPoint.Y - oldTranslateY) * scale;
                _isZooming = false;
            }
            else
            {
                // Zoom to center
                var centerX = Bounds.Width / 2;
                var centerY = Bounds.Height / 2;
                var oldTranslateX = TranslateX;
                var oldTranslateY = TranslateY;

                _isZooming = true;
                Zoom = newZoom;

                var scale = newZoom / (Zoom / (1 + delta));
                TranslateX = centerX - (centerX - oldTranslateX) * scale;
                TranslateY = centerY - (centerY - oldTranslateY) * scale;
                _isZooming = false;
            }
        }

        private void OnPointerPressedHandler(object? sender, PointerPressedEventArgs e)
        {
            if (!IsPanEnabled || _presenter == null)
                return;

            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                _isDragging = true;
                _lastMousePosition = e.GetCurrentPoint(this).Position;
                e.Pointer.Capture(this);
                e.Handled = true;
            }
        }

        private void OnPointerReleasedHandler(object? sender, PointerReleasedEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                e.Pointer.Capture(null);
                e.Handled = true;
            }
        }

        private void OnPointerMovedHandler(object? sender, PointerEventArgs e)
        {
            if (!_isDragging || _presenter == null)
                return;

            var currentPos = e.GetCurrentPoint(this).Position;
            var delta = currentPos - _lastMousePosition;

            TranslateX += delta.X;
            TranslateY += delta.Y;

            _lastMousePosition = currentPos;
            e.Handled = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Centers the content.
        /// </summary>
        public void CenterContent()
        {
            if (_presenter == null)
                return;

            var contentSize = _presenter.ContentSize;
            var viewSize = Bounds.Size;

            TranslateX = (viewSize.Width - contentSize.Width * Zoom) / 2;
            TranslateY = (viewSize.Height - contentSize.Height * Zoom) / 2;
        }

        /// <summary>
        /// Zooms to fill the available space.
        /// </summary>
        public void ZoomToFill()
        {
            if (_presenter == null)
                return;

            var contentSize = _presenter.ContentSize;
            var viewSize = Bounds.Size;

            var scaleX = viewSize.Width / contentSize.Width;
            var scaleY = viewSize.Height / contentSize.Height;
            var newZoom = Math.Min(scaleX, scaleY);

            _isZooming = true;
            Zoom = Math.Clamp(newZoom, MinZoom, MaxZoom);
            _isZooming = false;

            CenterContent();
            Mode = ZoomControlModes.Fill;
        }

        /// <summary>
        /// Resets the zoom to original size.
        /// </summary>
        public void ResetZoom()
        {
            Zoom = 1.0;
            TranslateX = 0;
            TranslateY = 0;
            Mode = ZoomControlModes.Original;
        }

        #endregion

        #region Private Methods

        private void UpdateViewport()
        {
            // Can be extended to update ViewFinder if implemented
        }

        #endregion
    }
}