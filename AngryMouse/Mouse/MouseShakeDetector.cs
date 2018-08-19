using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;

namespace AngryMouse.Mouse
{
    /// <summary>
    /// Detects mouse shaking using the global mouse hook.
    /// </summary>
    class MouseShakeDetector : IDisposable
    {
        /// <summary>
        /// Minimum milliseconds between recording a mouse event.
        /// </summary>
        private const int MouseEventRate = 10;

        /// <summary>
        /// Minimum tracked mouse movement time needed to detect a shake.
        /// </summary>
        private const int TrackingInterval = 500;

        /// <summary>
        /// Minimum speed required to detect a shake.
        /// </summary>
        private const double MinimumSpeed = 1.0;

        /// <summary>
        /// Minimum sharp turns required to detect a shake (negative dot product).
        /// </summary>
        private const int MinimumTurns = 4;

        /// <summary>
        /// The hook into mouse events
        /// </summary>
        private readonly IKeyboardMouseEvents mouseEvents;

        /// <summary>
        /// The last time we received a mouse event.
        /// </summary>
        private int lastMouseEvent = 0;

        /// <summary>
        /// Stores the recorded mouse positions.
        /// </summary>
        private LinkedList<MousePosition> mousePositions = new LinkedList<MousePosition>();

        /// <summary>
        /// Indicates whether the mouse is currently shaking or not.
        /// </summary>
        private bool shaking = false;

        /// <summary>
        /// Handler for mouse shaking events.
        /// </summary>
        public event EventHandler<MouseShakeArgs> MouseShake;

        /// <summary>
        /// Handler for mouse movement events.
        /// </summary>
        public event EventHandler<MouseEventExtArgs> MouseMove;

        /// <summary>
        /// Main constructor.
        /// </summary>
        public MouseShakeDetector()
        {
            mouseEvents = Hook.GlobalEvents();

            mouseEvents.MouseMoveExt += OnMouseMove;
        }

        /// <summary>
        /// Global hook callback.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">parameters of the mouse</param>
        private void OnMouseMove(object sender, MouseEventExtArgs e)
        {
            if (e.Timestamp - MouseEventRate > lastMouseEvent)
            {
                MouseMove?.Invoke(this, e);

                lastMouseEvent = e.Timestamp;

                while (mousePositions.Count > 0 &&
                    e.Timestamp - TrackingInterval > mousePositions.Last.Value.Timestamp)
                {
                    // Remove old positions
                    mousePositions.RemoveLast();
                }

                mousePositions.AddFirst(e);

                bool shaking = IsShaking();

                if (this.shaking != shaking)
                {
                    this.shaking = shaking;
                    MouseShakeArgs args = new MouseShakeArgs(shaking);
                    MouseShake?.Invoke(this, args);
                }
            }
        }

        /// <summary>
        /// Check the list of mouse positions to see if the mouse was shaking or not.
        /// </summary>
        /// <returns></returns>
        private bool IsShaking()
        {
            // At least 10 positions needed
            if (mousePositions.Count < 10)
            {
                return false;
            }

            double speedSum = 0;
            int sharpTurns = 0;

            LinkedListNode<MousePosition> current = mousePositions.First;

            // Loop throught the linked list, skipping the last element
            while (current.Next != null)
            {
                MousePosition p1 = current.Value;
                MousePosition p2 = current.Next.Value;
                MousePosition p0 = current.Previous == null ? null : current.Previous.Value;

                // Distance between the current and the next point.
                double d = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

                // Speed between the current and the next point.
                double v = d / (p1.Timestamp - p2.Timestamp);

                speedSum += v;

                // Check the movement angle in the point
                if (p0 != null && p1.Dot(p0, p2) < 0)
                {
                    sharpTurns++;
                }

                current = current.Next;
            }

            // Average mouse speed
            double avgSpeed = speedSum / (mousePositions.Count - 1);

            return avgSpeed >= MinimumSpeed && sharpTurns >= MinimumTurns;
        }

        public void Dispose()
        {
            mouseEvents.MouseMoveExt -= OnMouseMove;
        }
    }
}
