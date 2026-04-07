using System;

namespace GraphX.Controls.Models.Interfaces
{
    /// <summary>
    /// Factory interface for creating label controls.
    /// </summary>
    /// <typeparam name="TResult">The type of label control to create</typeparam>
    public interface ILabelFactory<TResult>
    {
        /// <summary>
        /// Creates a label for the specified content.
        /// </summary>
        /// <param name="content">The content to create a label for</param>
        /// <param name="id">Optional identifier for the label</param>
        /// <returns>The created label control</returns>
        TResult? CreateLabel(object? content, object? id = null);
    }
}