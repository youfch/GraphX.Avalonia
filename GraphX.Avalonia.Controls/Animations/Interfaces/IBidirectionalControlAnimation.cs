namespace GraphX.Controls.Animations
{
    /// <summary>
    /// Interface for bidirectional control animations.
    /// </summary>
    public interface IBidirectionalControlAnimation
    {
        /// <summary>
        /// Animation duration in milliseconds.
        /// </summary>
        double Duration { get; set; }

        /// <summary>
        /// Animate vertex forward.
        /// </summary>
        void AnimateVertexForward(VertexControl target);

        /// <summary>
        /// Animate vertex backward.
        /// </summary>
        void AnimateVertexBackward(VertexControl target);

        /// <summary>
        /// Animate edge forward.
        /// </summary>
        void AnimateEdgeForward(EdgeControl target);

        /// <summary>
        /// Animate edge backward.
        /// </summary>
        void AnimateEdgeBackward(EdgeControl target);
    }
}