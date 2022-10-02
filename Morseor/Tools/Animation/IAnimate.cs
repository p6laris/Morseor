namespace Morseor.Tools.Animation
{
    /// <summary>
    /// MorseAnimate blinking abstractions.
    /// </summary>

    public interface IAnimate
    {
        /// <summary>
        /// Animates the morse code based on dash and dots.
        /// </summary>
        /// <returns><see cref="Task{TResult]"/></returns>
        Task AnimateMorse();
    }
}
