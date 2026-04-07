namespace GraphX.Controls.Models.Interfaces
{
    /// <summary>
    /// Interface for controls that can be attached to other controls.
    /// Used for labels that attach to vertices or edges.
    /// </summary>
    public interface IAttachableControl
    {
        /// <summary>
        /// Attaches this control to the specified target control.
        /// </summary>
        /// <param name="control">The control to attach to</param>
        void Attach(object? control);

        /// <summary>
        /// Detaches this control from its current target.
        /// </summary>
        void Detach();
    }

    /// <summary>
    /// Generic interface for controls that can be attached to other controls.
    /// </summary>
    /// <typeparam name="T">The type of control to attach to</typeparam>
    public interface IAttachableControl<in T> : IAttachableControl where T : class
    {
        /// <summary>
        /// Attaches this control to the specified target control.
        /// </summary>
        /// <param name="control">The control to attach to</param>
        new void Attach(T? control);
    }
}