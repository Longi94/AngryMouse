using Gma.System.MouseKeyHook;
using System.Numerics;

namespace AngryMouse.Mouse
{
    /// <summary>
    /// Represents a position of the cursor including the timestamp.
    /// </summary>
    public class MousePosition
    {
        /// <summary>
        /// The position
        /// </summary>
        public readonly Vector2 Position;

        /// <summary>
        /// X positon of the mouse.
        /// </summary>
        public float X
        {
            get
            {
                return Position.X;
            }
        }

        /// <summary>
        /// Y position of the mouse.
        /// </summary>
        public float Y
        {
            get
            {
                return Position.Y;
            }
        }

        /// <summary>
        /// The timestamp when the mouse was in this position.
        /// </summary>
        public int Timestamp { get; }

        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="timestamp">the timestamp when the mouse was in this position</param>
        public MousePosition(int x, int y, int timestamp)
        {
            Position = new Vector2(x, y);
            Timestamp = timestamp;
        }

        /// <summary>
        /// Calculate the dot product of the vectors that point from before to here and from here to after.
        /// </summary>
        /// <param name="before">Previous position.</param>
        /// <param name="after">Next position.</param>
        /// <returns></returns>
        public float Dot(MousePosition before, MousePosition after)
        {
            var v1 = Position - before.Position;
            var v2 = after.Position - Position;

            return Vector2.Dot(v1, v2);
        }

        /// <summary>
        /// Convert a <see cref="MouseEventExtArgs"/> object to a <see cref="MousePosition"/> object.
        /// </summary>
        /// <param name="e"></param>
        public static implicit operator MousePosition(MouseEventExtArgs e) =>
            new MousePosition(e.X, e.Y, e.Timestamp);
    }
}
