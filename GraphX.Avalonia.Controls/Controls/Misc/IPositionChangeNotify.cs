namespace GraphX.Controls
{
    /// <summary>
    /// Interface for controls that need to be notified of position changes.
    /// </summary>
    public interface IPositionChangeNotify
    {
        /// <summary>
        /// Called when the position changes.
        /// </summary>
        void OnPositionChanged();
    }
}