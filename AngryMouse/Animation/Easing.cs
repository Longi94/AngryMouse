namespace AngryMouse.Animation
{
    /// <summary>
    /// Easing functions.
    /// </summary>
    public class Easing
    {
        /// <summary>
        /// Cubic in-out easing.
        /// </summary>
        /// <param name="t">Animation progress. Has to be in range [0,1].</param>
        /// <returns>Easing progress. Will be in range [0,1].</returns>
        public static double CubicInOut(double t)
        {
            return t < .5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
        }
    }
}
