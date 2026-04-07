using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using GraphX.Common.Enums;

namespace GraphX.Controls
{
    public static class HighlightBehaviour
    {
        #region Attached props
        //trigger
        public static readonly AttachedProperty<bool> HighlightedProperty =
            AvaloniaProperty.RegisterAttached<HighlightBehaviour, Control, bool>("Highlighted", false);

        //settings
        public static readonly AttachedProperty<bool> IsHighlightEnabledProperty =
            AvaloniaProperty.RegisterAttached<HighlightBehaviour, Control, bool>("IsHighlightEnabled", false,
                inherited: true,
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: OnIsHighlightEnabledPropertyChanged);

        public static readonly AttachedProperty<GraphControlType> HighlightControlProperty =
            AvaloniaProperty.RegisterAttached<HighlightBehaviour, Control, GraphControlType>("HighlightControl", GraphControlType.VertexAndEdge);

        public static readonly AttachedProperty<EdgesType> HighlightEdgesProperty =
            AvaloniaProperty.RegisterAttached<HighlightBehaviour, Control, EdgesType>("HighlightEdges", EdgesType.Out);

        public static readonly AttachedProperty<HighlightedEdgeType> HighlightedEdgeTypeProperty =
            AvaloniaProperty.RegisterAttached<HighlightBehaviour, Control, HighlightedEdgeType>("HighlightedEdgeType", HighlightedEdgeType.None);

        public static HighlightedEdgeType GetHighlightedEdgeType(AvaloniaObject obj)
        {
            return obj.GetValue(HighlightedEdgeTypeProperty);
        }

        public static void SetHighlightedEdgeType(AvaloniaObject obj, HighlightedEdgeType value)
        {
            obj.SetValue(HighlightedEdgeTypeProperty, value);
        }

        public static bool GetIsHighlightEnabled(AvaloniaObject obj)
        {
            return obj.GetValue(IsHighlightEnabledProperty);
        }

        public static void SetIsHighlightEnabled(AvaloniaObject obj, bool value)
        {
            obj.SetValue(IsHighlightEnabledProperty, value);
        }

        public static bool GetHighlighted(AvaloniaObject obj)
        {
            return obj.GetValue(HighlightedProperty);
        }

        public static void SetHighlighted(AvaloniaObject obj, bool value)
        {
            obj.SetValue(HighlightedProperty, value);
        }

        public static GraphControlType GetHighlightControl(AvaloniaObject obj)
        {
            return obj.GetValue(HighlightControlProperty);
        }

        public static void SetHighlightControl(AvaloniaObject obj, GraphControlType value)
        {
            obj.SetValue(HighlightControlProperty, value);
        }

        public static EdgesType GetHighlightEdges(AvaloniaObject obj)
        {
            return obj.GetValue(HighlightEdgesProperty);
        }

        public static void SetHighlightEdges(AvaloniaObject obj, EdgesType value)
        {
            obj.SetValue(HighlightEdgesProperty, value);
        }

        #endregion

        #region PropertyChanged callbacks
        private static void OnIsHighlightEnabledPropertyChanged(AvaloniaObject obj, AvaloniaPropertyChangedEventArgs e)
        {
            var element = obj as IInputElement;
            if (element == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
            {
                //register the event handlers
                element.PointerEntered += element_PointerEntered;
                element.PointerExited += element_PointerExited;
            }
            else
            {
                //unregister the event handlers
                element.PointerEntered -= element_PointerEntered;
                element.PointerExited -= element_PointerExited;
            }
        }

        static void element_PointerExited(object sender, PointerEventArgs e)
        {
            if (sender is AvaloniaObject == false) return;
            var ctrl = sender as IGraphControl;
            if (ctrl == null) return;

            var type = GetHighlightControl(sender as AvaloniaObject);
            var edgesType = GetHighlightEdges(sender as AvaloniaObject);
            SetHighlighted(sender as AvaloniaObject, false);

            if (type == GraphControlType.Vertex || type == GraphControlType.VertexAndEdge)
                foreach (var item in ctrl.RootArea.GetRelatedVertexControls(ctrl, edgesType).Cast<AvaloniaObject>())
                    SetHighlighted(item, false);

            if (type == GraphControlType.Edge || type == GraphControlType.VertexAndEdge)
                foreach (var item in ctrl.RootArea.GetRelatedEdgeControls(ctrl, edgesType).Cast<AvaloniaObject>())
                {
                    SetHighlighted(item, false);
                    SetHighlightedEdgeType(item, HighlightedEdgeType.None);
                }
        }

        static void element_PointerEntered(object sender, PointerEventArgs e)
        {
            if (sender is AvaloniaObject == false) return;
            var ctrl = sender as IGraphControl;
            if (ctrl == null) return;

            var type = GetHighlightControl(sender as AvaloniaObject);
            var edgesType = GetHighlightEdges(sender as AvaloniaObject);
            SetHighlighted(sender as AvaloniaObject, true);

            //highlight related vertices
            if (type == GraphControlType.Vertex || type == GraphControlType.VertexAndEdge)
                foreach (var item in ctrl.RootArea.GetRelatedVertexControls(ctrl, edgesType).Cast<AvaloniaObject>())
                    SetHighlighted(item, true);
            //highlight related edges
            if (type == GraphControlType.Edge || type == GraphControlType.VertexAndEdge)
            {
                //separately get in and out edges to set direction flag
                if (edgesType == EdgesType.In || edgesType == EdgesType.All)
                    foreach (var item in ctrl.RootArea.GetRelatedEdgeControls(ctrl, EdgesType.In).Cast<AvaloniaObject>())
                    {
                        SetHighlighted(item, true);
                        SetHighlightedEdgeType(item, HighlightedEdgeType.In);
                    }
                if (edgesType == EdgesType.Out || edgesType == EdgesType.All)
                    foreach (var item in ctrl.RootArea.GetRelatedEdgeControls(ctrl, EdgesType.Out).Cast<AvaloniaObject>())
                    {
                        SetHighlighted(item, true);
                        SetHighlightedEdgeType(item, HighlightedEdgeType.Out);
                    }
            }
        }
        #endregion

        public enum HighlightType
        {
            Vertex,
            Edge,
            VertexAndEdge
        }

        public enum HighlightedEdgeType
        {
            In,
            Out,
            None
        }
    }
}
