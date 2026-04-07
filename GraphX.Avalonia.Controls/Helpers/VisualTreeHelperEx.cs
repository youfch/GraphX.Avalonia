using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Visuals;

namespace GraphX.Controls
{
    public static class VisualTreeHelperEx
    {
        public static T? FindAncestorByType<T>(AvaloniaObject? depObj) where T : AvaloniaObject
        {
            if (depObj == null)
            {
                return default(T);
            }
            if (depObj is T)
            {
                return (T)depObj;
            }

            T? parent = default(T);
            var parentObj = depObj.GetVisualParent();
            if (parentObj != null)
                parent = VisualTreeHelperEx.FindAncestorByType<T>(parentObj);

            return parent;
        }

        public static AvaloniaObject? FindDescendantByName(AvaloniaObject element, string name)
        {
            if (element != null && (element is Control) && (element as Control)?.Name == name)
                return element;

            AvaloniaObject? foundElement = null;

            if (element is Control control)
            {
                foreach (var child in control.GetVisualChildren())
                {
                    if (child is AvaloniaObject visual)
                    {
                        foundElement = VisualTreeHelperEx.FindDescendantByName(visual, name);
                        if (foundElement != null)
                            break;
                    }
                }
            }

            return foundElement;
        }

        public static T? FindDescendantByType<T>(AvaloniaObject? element) where T : AvaloniaObject
        {
            if (element == null)
                return null;

            if (element is T)
                return (T)element;

            if (element is Control container)
            {
                foreach (var child in container.GetVisualChildren())
                {
                    if (child is AvaloniaObject visual)
                    {
                        var found = VisualTreeHelperEx.FindDescendantByType<T>(visual);
                        if (found != null)
                            return found;
                    }
                }
            }

            return null;
        }

        public static IEnumerable<T> FindDescendantsOfType<T>(this AvaloniaObject? element) where T : class
        {
            if (element == null) yield break;
            
            if (element is T matchingElement)
                yield return matchingElement;

            if (element is Control container)
            {
                foreach (var child in container.GetVisualChildren())
                {
                    if (child is AvaloniaObject visual)
                    {
                        foreach (var item in visual.FindDescendantsOfType<T>())
                            yield return item;
                    }
                }
            }
        }
    }
}
