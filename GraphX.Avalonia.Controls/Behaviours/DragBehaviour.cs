using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using GraphX.Common.Exceptions;
using GraphX.Common.Interfaces;
using GraphX.Common.Models;

namespace GraphX.Controls
{
    public static class DragBehaviour
    {
        public delegate double SnapModifierFunc(GraphAreaBase area, AvaloniaObject obj, double val);

        #region Built-in snapping behavior

        private static readonly Func<AvaloniaObject, bool> _builtinIsSnappingPredicate = obj =>
        {
            var keys = Keyboard.GetKeyModifiers();
            return keys.HasFlag(KeyModifiers.Shift);
        };

        private static readonly Func<AvaloniaObject, bool> _builtinIsIndividualSnappingPredicate = obj => false;

        private static readonly SnapModifierFunc _builtinSnapModifier = (area, obj, val) => Math.Round(val * 0.1) * 10.0;

        #endregion Built-in snapping behavior

        #region Global snapping behavior management

        private static Func<AvaloniaObject, bool> _globalIsSnappingPredicate = _builtinIsSnappingPredicate;

        public static Func<AvaloniaObject, bool> GlobalIsSnappingPredicate
        {
            get => _globalIsSnappingPredicate;
            set => _globalIsSnappingPredicate = value ?? _builtinIsSnappingPredicate;
        }

        private static Func<AvaloniaObject, bool> _globalIsIndividualSnappingPredicate = _builtinIsIndividualSnappingPredicate;

        public static Func<AvaloniaObject, bool> GlobalIsIndividualSnappingPredicate
        {
            get => _globalIsIndividualSnappingPredicate;
            set => _globalIsIndividualSnappingPredicate = value ?? _builtinIsIndividualSnappingPredicate;
        }

        private static SnapModifierFunc _globalXSnapModifier = _builtinSnapModifier;

        public static SnapModifierFunc GlobalXSnapModifier
        {
            get => _globalXSnapModifier;
            set => _globalXSnapModifier = value ?? _builtinSnapModifier;
        }

        private static SnapModifierFunc _globalYSnapModifier = _builtinSnapModifier;

        public static SnapModifierFunc GlobalYSnapModifier
        {
            get => _globalYSnapModifier;
            set => _globalYSnapModifier = value ?? _builtinSnapModifier;
        }

        #endregion Global snapping behavior management

        #region Attached Properties

        public static readonly AttachedProperty<bool> IsDragEnabledProperty =
            AvaloniaProperty.RegisterAttached<DragBehaviour, Control, bool>("IsDragEnabled", false,
                inheritanceHook: OnIsDragEnabledPropertyChanged);

        public static readonly AttachedProperty<bool> UpdateEdgesOnMoveProperty =
            AvaloniaProperty.RegisterAttached<DragBehaviour, Control, bool>("UpdateEdgesOnMove", false);

        public static readonly AttachedProperty<bool> IsTaggedProperty =
            AvaloniaProperty.RegisterAttached<DragBehaviour, Control, bool>("IsTagged", false);

        public static readonly AttachedProperty<bool> IsDraggingProperty =
            AvaloniaProperty.RegisterAttached<DragBehaviour, Control, bool>("IsDragging", false);

        public static readonly AttachedProperty<Func<AvaloniaObject, bool>?> IsSnappingPredicateProperty =
            AvaloniaProperty.RegisterAttached<DragBehaviour, Control, Func<AvaloniaObject, bool>?>(
                "IsSnappingPredicate", null);

        public static readonly AttachedProperty<Func<AvaloniaObject, bool>?> IsIndividualSnappingPredicateProperty =
            AvaloniaProperty.RegisterAttached<DragBehaviour, Control, Func<AvaloniaObject, bool>?>(
                "IsIndividualSnappingPredicate", null);

        public static readonly AttachedProperty<SnapModifierFunc?> XSnapModifierProperty =
            AvaloniaProperty.RegisterAttached<DragBehaviour, Control, SnapModifierFunc?>(
                "XSnapModifier", null);

        public static readonly AttachedProperty<SnapModifierFunc?> YSnapModifierProperty =
            AvaloniaProperty.RegisterAttached<DragBehaviour, Control, SnapModifierFunc?>(
                "YSnapModifier", null);

        private static readonly AttachedProperty<double> OriginalXProperty =
            AvaloniaProperty.RegisterAttached<DragBehaviour, Control, double>("OriginalX", 0.0);

        private static readonly AttachedProperty<double> OriginalYProperty =
            AvaloniaProperty.RegisterAttached<DragBehaviour, Control, double>("OriginalY", 0.0);

        private static readonly AttachedProperty<double> OriginalMouseXProperty =
            AvaloniaProperty.RegisterAttached<DragBehaviour, Control, double>("OriginalMouseX", 0.0);

        private static readonly AttachedProperty<double> OriginalMouseYProperty =
            AvaloniaProperty.RegisterAttached<DragBehaviour, Control, double>("OriginalMouseY", 0.0);

        #endregion Attached Properties

        #region Get/Set methods for Attached Properties

        public static bool GetUpdateEdgesOnMove(Control obj)
        {
            return obj.GetValue(UpdateEdgesOnMoveProperty);
        }

        public static void SetUpdateEdgesOnMove(Control obj, bool value)
        {
            obj.SetValue(UpdateEdgesOnMoveProperty, value);
        }

        public static bool GetIsTagged(Control obj)
        {
            return obj.GetValue(IsTaggedProperty);
        }

        public static void SetIsTagged(Control obj, bool value)
        {
            obj.SetValue(IsTaggedProperty, value);
        }

        public static bool GetIsDragEnabled(Control obj)
        {
            return obj.GetValue(IsDragEnabledProperty);
        }

        public static void SetIsDragEnabled(Control obj, bool value)
        {
            obj.SetValue(IsDragEnabledProperty, value);
        }

        public static bool GetIsDragging(Control obj)
        {
            return obj.GetValue(IsDraggingProperty);
        }

        public static void SetIsDragging(Control obj, bool value)
        {
            obj.SetValue(IsDraggingProperty, value);
        }

        public static Func<AvaloniaObject, bool>? GetIsSnappingPredicate(Control obj)
        {
            return obj.GetValue(IsSnappingPredicateProperty);
        }

        public static void SetIsSnappingPredicate(Control obj, Func<AvaloniaObject, bool>? value)
        {
            obj.SetValue(IsSnappingPredicateProperty, value);
        }

        public static Func<AvaloniaObject, bool>? GetIsIndividualSnappingPredicate(Control obj)
        {
            return obj.GetValue(IsIndividualSnappingPredicateProperty);
        }

        public static void SetIsIndividualSnappingPredicate(Control obj, Func<AvaloniaObject, bool>? value)
        {
            obj.SetValue(IsIndividualSnappingPredicateProperty, value);
        }

        public static SnapModifierFunc? GetXSnapModifier(Control obj)
        {
            return obj.GetValue(XSnapModifierProperty);
        }

        public static void SetXSnapModifier(Control obj, SnapModifierFunc? value)
        {
            obj.SetValue(XSnapModifierProperty, value);
        }

        public static SnapModifierFunc? GetYSnapModifier(Control obj)
        {
            return obj.GetValue(YSnapModifierProperty);
        }

        public static void SetYSnapModifier(Control obj, SnapModifierFunc? value)
        {
            obj.SetValue(YSnapModifierProperty, value);
        }

        #endregion Get/Set methods for Attached Properties

        #region Get/Set methods for private Attached Properties

        private static double GetOriginalX(AvaloniaObject obj)
        {
            return obj.GetValue(OriginalXProperty);
        }

        private static void SetOriginalX(AvaloniaObject obj, double value)
        {
            obj.SetValue(OriginalXProperty, value);
        }

        private static double GetOriginalY(AvaloniaObject obj)
        {
            return obj.GetValue(OriginalYProperty);
        }

        private static void SetOriginalY(AvaloniaObject obj, double value)
        {
            obj.SetValue(OriginalYProperty, value);
        }

        private static double GetOriginalMouseX(AvaloniaObject obj)
        {
            return obj.GetValue(OriginalMouseXProperty);
        }

        private static void SetOriginalMouseX(AvaloniaObject obj, double value)
        {
            obj.SetValue(OriginalMouseXProperty, value);
        }

        private static double GetOriginalMouseY(AvaloniaObject obj)
        {
            return obj.GetValue(OriginalMouseYProperty);
        }

        private static void SetOriginalMouseY(AvaloniaObject obj, double value)
        {
            obj.SetValue(OriginalMouseYProperty, value);
        }

        #endregion Get/Set methods for private Attached Properties

        #region PropertyChanged callbacks

        private static void OnIsDragEnabledPropertyChanged(AvaloniaObject obj, AvaloniaPropertyChangedEventArgs e)
        {
            var element = obj as Control;
            if (element == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
            {
                if (element is VertexControl vertexControl)
                {
                    vertexControl.PointerPressed += OnVertexDragStarted;
                    vertexControl.PointerReleased += OnVertexDragFinished;
                }
                else if (element is EdgeControl edgeControl)
                {
                    edgeControl.PointerPressed += OnEdgeDragStarted;
                    edgeControl.PointerReleased += OnEdgeDragFinished;
                }
            }
            else
            {
                if (element is VertexControl vertexControl)
                {
                    vertexControl.PointerPressed -= OnVertexDragStarted;
                    vertexControl.PointerReleased -= OnVertexDragFinished;
                }
                else if (element is EdgeControl edgeControl)
                {
                    edgeControl.PointerPressed -= OnEdgeDragStarted;
                    edgeControl.PointerReleased -= OnEdgeDragFinished;
                }
            }
        }

        #endregion PropertyChanged callbacks

        private static void OnEdgeDragStarted(object? sender, PointerPressedEventArgs e)
        {
            var obj = sender as AvaloniaObject;
            if (obj == null) return;

            SetIsDragging(obj, true);

            var element = sender as Control;
            if (element != null)
            {
                element.CapturePointer(e.Pointer);
                element.PointerMoved -= OnEdgeDragging;
                element.PointerMoved += OnEdgeDragging;
            }
            else throw new GX_InvalidDataException("The control must be a descendent of Control!");

            e.Handled = false;
        }

        private static void OnEdgeDragging(object? sender, PointerEventArgs e)
        {
            var obj = sender as AvaloniaObject;
            if (obj == null || !GetIsDragging(obj))
                return;

            var edgeControl = sender as EdgeControl;
            edgeControl?.PrepareEdgePathFromMousePointer();

            e.Handled = true;
        }

        private static void OnEdgeDragFinished(object? sender, PointerReleasedEventArgs e)
        {
            var edgeControl = sender as EdgeControl;
            if (edgeControl == null) return;

            var graphAreaBase = edgeControl.RootArea;
            var position = e.GetCurrentPoint(graphAreaBase).Position;
            var vertexControl = graphAreaBase?.GetVertexControlAt(position);

            if (vertexControl != null)
            {
                edgeControl.Target = vertexControl;

                if (vertexControl.VertexConnectionPointsList.Count > 0)
                {
                    var vertexConnectionPoint = vertexControl.GetConnectionPointAt(position);
                    var edge = edgeControl.Edge as IGraphXCommonEdge;

                    if (vertexConnectionPoint != null)
                    {
                        edge.TargetConnectionPointId = vertexConnectionPoint.Id;
                    }
                    else
                    {
                        edge.TargetConnectionPointId = null;
                    }
                }

                edgeControl.UpdateEdge();

                var obj = sender as AvaloniaObject;
                SetIsDragging(obj!, false);

                var element = sender as Control;
                if (element != null)
                {
                    element.PointerMoved -= OnEdgeDragging;
                    element.ReleasePointerCapture(e.Pointer);
                }
            }
        }

        private static void OnVertexDragStarted(object? sender, PointerPressedEventArgs e)
        {
            var obj = sender as AvaloniaObject;
            if (obj == null) return;

            SetIsDragging(obj, true);

            var area = GetAreaFromObject(obj);
            var pos = GetPositionInArea(area, e);
            SetOriginalMouseX(obj, pos.X);
            SetOriginalMouseY(obj, pos.Y);

            SetOriginalX(obj, GraphAreaBase.GetFinalX(obj));
            SetOriginalY(obj, GraphAreaBase.GetFinalY(obj));

            if (area != null)
            {
                foreach (var item in area.GetAllVertexControls())
                    if (!ReferenceEquals(item, obj) && GetIsTagged(item))
                    {
                        SetOriginalX(item, GraphAreaBase.GetFinalX(item));
                        SetOriginalY(item, GraphAreaBase.GetFinalY(item));
                    }
            }

            var element = sender as Control;
            if (element != null)
            {
                element.CapturePointer(e.Pointer);
                element.PointerMoved -= OnVertexDragging;
                element.PointerMoved += OnVertexDragging;
            }

            e.Handled = false;
        }

        private static void OnVertexDragFinished(object? sender, PointerReleasedEventArgs e)
        {
            UpdateVertexEdges(sender as VertexControl);

            var obj = sender as AvaloniaObject;
            if (obj == null) return;

            SetIsDragging(obj, false);
            obj.ClearValue(OriginalMouseXProperty);
            obj.ClearValue(OriginalMouseYProperty);
            obj.ClearValue(OriginalXProperty);
            obj.ClearValue(OriginalYProperty);

            if (GetIsTagged(obj))
            {
                var area = GetAreaFromObject(obj);
                if (area != null)
                {
                    foreach (var item in area.GetAllVertexControls())
                        if (GetIsTagged(item))
                        {
                            item.ClearValue(OriginalXProperty);
                            item.ClearValue(OriginalYProperty);
                        }
                }
            }

            var element = sender as Control;
            if (element != null)
            {
                element.PointerMoved -= OnVertexDragging;
                element.ReleasePointerCapture(e.Pointer);
            }
        }

        private static void OnVertexDragging(object? sender, PointerEventArgs e)
        {
            var obj = sender as AvaloniaObject;
            if (obj == null || !GetIsDragging(obj))
                return;

            var area = GetAreaFromObject(obj);
            var pos = GetPositionInArea(area, e);

            double horizontalChange = pos.X - GetOriginalMouseX(obj);
            double verticalChange = pos.Y - GetOriginalMouseY(obj);

            bool snap = GetIsSnappingPredicate(obj)?.Invoke(obj) ?? _globalIsSnappingPredicate(obj);
            bool individualSnap = false;
            SnapModifierFunc? snapXMod = null;
            SnapModifierFunc? snapYMod = null;
            SnapModifierFunc? individualSnapXMod = null;
            SnapModifierFunc? individualSnapYMod = null;

            if (snap)
            {
                snapXMod = GetXSnapModifier(obj) ?? _globalXSnapModifier;
                snapYMod = GetYSnapModifier(obj) ?? _globalYSnapModifier;
                individualSnap = GetIsIndividualSnappingPredicate(obj)?.Invoke(obj) ?? _globalIsIndividualSnappingPredicate(obj);
                if (individualSnap)
                {
                    individualSnapXMod = snapXMod;
                    individualSnapYMod = snapYMod;
                }
            }

            if (GetIsTagged(obj))
            {
                var primaryDragVertex = obj as VertexControl;
                if (primaryDragVertex == null)
                {
                    var ec = obj as EdgeControl;
                    if (ec != null)
                        primaryDragVertex = ec.Source ?? ec.Target;

                    if (primaryDragVertex == null)
                    {
                        Debug.WriteLine("OnDragging() -> Tagged and dragged the wrong object?");
                        return;
                    }
                }

                UpdateCoordinates(area, primaryDragVertex, horizontalChange, verticalChange, snapXMod, snapYMod);

                if (!individualSnap)
                {
                    horizontalChange = GraphAreaBase.GetFinalX(primaryDragVertex) - GetOriginalX(primaryDragVertex);
                    verticalChange = GraphAreaBase.GetFinalY(primaryDragVertex) - GetOriginalY(primaryDragVertex);
                }

                if (area != null)
                {
                    foreach (var item in area.GetAllVertexControls())
                        if (!ReferenceEquals(item, primaryDragVertex) && GetIsTagged(item))
                            UpdateCoordinates(area, item, horizontalChange, verticalChange, individualSnapXMod, individualSnapYMod);
                }
            }
            else
            {
                UpdateCoordinates(area, obj, horizontalChange, verticalChange, snapXMod, snapYMod);
            }

            e.Handled = true;
        }

        private static void UpdateVertexEdges(VertexControl? vc)
        {
            if (vc?.Vertex != null)
            {
                var ra = vc.RootArea;
                if (ra == null) throw new GX_InvalidDataException("OnDragFinished() - IGraphControl object must always have RootArea property set!");
                if (ra.IsEdgeRoutingEnabled)
                {
                    ra.ComputeEdgeRoutesByVertex(vc);
                    vc.InvalidateVisual();
                }
            }
        }

        private static void UpdateCoordinates(GraphAreaBase? area, AvaloniaObject obj, double horizontalChange, double verticalChange, SnapModifierFunc? xSnapModifier, SnapModifierFunc? ySnapModifier)
        {
            if (area == null) return;

            if (double.IsNaN(GraphAreaBase.GetX(obj)))
                GraphAreaBase.SetX(obj, 0, true);
            if (double.IsNaN(GraphAreaBase.GetY(obj)))
                GraphAreaBase.SetY(obj, 0, true);

            var x = GetOriginalX(obj) + horizontalChange;
            if (xSnapModifier != null)
                x = xSnapModifier(area, obj, x);
            GraphAreaBase.SetX(obj, x, true);

            var y = GetOriginalY(obj) + verticalChange;
            if (ySnapModifier != null)
                y = ySnapModifier(area, obj, y);
            GraphAreaBase.SetY(obj, y, true);

            if (GetUpdateEdgesOnMove(obj) && obj is VertexControl vc)
                UpdateVertexEdges(vc);
        }

        private static Point GetPositionInArea(GraphAreaBase? area, PointerEventArgs e)
        {
            if (area != null)
            {
                return e.GetCurrentPoint(area).Position;
            }
            throw new GX_InvalidDataException("DragBehavior.GetPositionInArea() - The input element must be a child of a GraphAreaBase.");
        }

        private static GraphAreaBase? GetAreaFromObject(object obj)
        {
            GraphAreaBase? area = null;

            if (obj is VertexControl vc)
                area = vc.RootArea;
            else if (obj is EdgeControl ec)
                area = ec.RootArea;
            else if (obj is AvaloniaObject avaloniaObj)
                area = VisualTreeHelperEx.FindAncestorByType<GraphAreaBase>(avaloniaObj);

            return area;
        }
    }
}
