namespace GraphX.Controls
{
    /// <summary>
    /// Defines the action to take when LogicCore property changes.
    /// </summary>
    public enum LogicCoreChangedAction
    {
        None = 0,
        GenerateGraph,
        GenerateGraphWithEdges,
        RelayoutGraph,
        RelayoutGraphWithEdges
    }
}