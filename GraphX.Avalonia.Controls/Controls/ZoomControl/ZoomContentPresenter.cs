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
    /// Zoom content presenter that tracks content size changes.
    /// </summary>
    public class ZoomContentPresenter : ContentControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Event for content size changes.
        /// </summary>
        public event Action<Size>? ContentSizeChanged;

        private Size _contentSize;

        /// <summary>
        /// Gets the content size.
        /// </summary>
        public Size ContentSize
        {
            get => _contentSize;
            private set
            {
                if (value == _contentSize)
                    return;
                _contentSize = value;
                ContentSizeChanged?.Invoke(value);
                OnPropertyChanged(nameof(ContentSize));
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var max = 1000000000;
            var x = double.IsInfinity(availableSize.Width) ? max : availableSize.Width;
            var y = double.IsInfinity(availableSize.Height) ? max : availableSize.Height;
            return new Size(x, y);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var child = Presenter?.Child;
            if (child == null)
                return finalSize;

            ContentSize = child.DesiredSize;
            child.Arrange(new Rect(child.DesiredSize));

            return finalSize;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}