using System;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace GraphX.Controls
{
    /// <summary>
    /// Custom helper methods for GraphX controls.
    /// </summary>
    public static class CustomHelper
    {
        public static bool IsIntegerInput(string text)
        {
            return text != "\r" && new Regex("[^0-9]+").IsMatch(text);
        }

        public static bool IsDoubleInput(string text)
        {
            return text != "\r" && new Regex("[^0-9.]+").IsMatch(text);
        }

        public static ScaleTransform? GetScaleTransform(Visual target)
        {
            if (target is not AvaloniaObject ao)
                return null;

            var transform = target.RenderTransform as ScaleTransform;
            return transform;
        }

        public static T? FindDescendantByName<T>(this Visual element, string name) where T : Visual
        {
            if (element == null || string.IsNullOrWhiteSpace(name))
                return null;

            if (element is IStyledElement styled && name.Equals(styled.Name, StringComparison.OrdinalIgnoreCase))
            {
                return element as T;
            }

            foreach (var child in element.GetVisualChildren())
            {
                var result = child.FindDescendantByName<T>(name);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static bool IsInDesignMode(AvaloniaObject? ctrl = null)
        {
            // In Avalonia, we can check if we're in design mode through the design-time properties
            // For now, return false as Avalonia's design-time detection is different from WPF
            if (ctrl != null)
            {
                // Could check Design.IsDesignMode attached property if needed
            }
            return false;
        }
    }
}