using GraphX.Controls.Models;

namespace GraphX.Controls.Animations
{
    /// <summary>
    /// Interface for one-way control animations.
    /// </summary>
    public interface IOneWayControlAnimation
    {
        /// <summary>
        /// Animation duration in milliseconds.
        /// </summary>
        double Duration { get; set; }

        /// <summary>
        /// Run vertex animation.
        /// </summary>
        /// <param name="target">Target vertex control</param>
        /// <param name="removeDataVertex">Remove data vertex from data graph when animation is finished</param>
        void AnimateVertex(VertexControl target, bool removeDataVertex = false);

        /// <summary>
        /// Run edge animation.
        /// </summary>
        /// <param name="target">Target edge control</param>
        /// <param name="removeDataEdge">Remove data edge from data graph when animation is finished</param>
        void AnimateEdge(EdgeControl target, bool removeDataEdge = false);

        /// <summary>
        /// Completed event that fires when animation is complete.
        /// </summary>
        event RemoveControlEventHandler Completed;
    }
}