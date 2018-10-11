using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;

namespace AngryMouse.Mouse
{
    /// <summary>
    /// Detects mouse shaking using the global mouse hook.
    /// </summary>
    public class MouseShakeDetector : IDisposable
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
        private readonly IKeyboardMouseEvents _mouseEvents;

        /// <summary>
        /// The last time we received a mouse event.
        /// </summary>
        private DateTime _lastMouseEvent = DateTime.MinValue;

        /// <summary>
        /// Stores the recorded mouse positions.
        /// </summary>
        private readonly LinkedList<MousePosition> _mousePositions = new LinkedList<MousePosition>();

        /// <summary>
        /// Indicates whether the mouse is currently shaking or not.
        /// </summary>
        private bool _shaking;

        /// <summary>
        /// Handler for mouse shaking events.
        /// </summary>
        public event EventHandler<MouseShakeArgs> MouseShake;

        /// <summary>
        /// Handler for mouse movement events.
        /// </summary>
        public event EventHandler<MouseEventExtArgs> MouseMove;

        /// <summary>
        /// Timer for hiding the mouse when it's not moving.
        /// </summary>
        private readonly Timer _timer = new Timer();

        /// <summary>
        /// Main constructor.
        /// </summary>
        public MouseShakeDetector()
        {
            _mouseEvents = Hook.GlobalEvents();

            _mouseEvents.MouseMoveExt += OnMouseMove;

            _timer.Interval = 100;
            _timer.Elapsed += Timer_Tick;
            _timer.Enabled = true;
        }

        /// <summary>
        /// Global hook callback.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">parameters of the mouse</param>
        private void OnMouseMove(object sender, MouseEventExtArgs e)
        {
            var currentTime = DateTime.Now;
            if (currentTime.AddMilliseconds(-MouseEventRate) > _lastMouseEvent)
            {
                MouseMove?.Invoke(this, e);

                _lastMouseEvent = currentTime;

                while (_mousePositions.Count > 0 &&
                       e.Timestamp - TrackingInterval > _mousePositions.Last.Value.Timestamp)
                {
                    // Remove old positions
                    _mousePositions.RemoveLast();
                }

                _mousePositions.AddFirst(e);

                SetShaking(IsShaking());
            }
        }

        /// <summary>
        /// Check the list of mouse positions to see if the mouse was shaking or not.
        /// </summary>
        /// <returns></returns>
        private bool IsShaking()
        {
            // At least 10 positions needed
            if (_mousePositions.Count < 10)
            {
                return false;
            }

            double speedSum = 0;
            int sharpTurns = 0;

            LinkedListNode<MousePosition> current = _mousePositions.First;

            // Loop thought the linked list, skipping the last element
            while (current.Next != null)
            {
                MousePosition p1 = current.Value;
                MousePosition p2 = current.Next.Value;
                MousePosition p0 = current.Previous?.Value;

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
            double avgSpeed = speedSum / (_mousePositions.Count - 1);

            return avgSpeed >= MinimumSpeed && sharpTurns >= MinimumTurns;
        }

        private void SetShaking(bool shaking)
        {
            if (_shaking != shaking)
            {
                _shaking = shaking;
                MouseShakeArgs args = new MouseShakeArgs(shaking, DateTime.Now);
                MouseShake?.Invoke(this, args);
            }
        }

        private void Timer_Tick(object sender, ElapsedEventArgs e)
        {
            if (_shaking && DateTime.Now.AddMilliseconds(-500) > _lastMouseEvent)
            {
                Application.Current.Dispatcher.Invoke(() => { SetShaking(false); });
            }
        }

        public void Dispose()
        {
            _mouseEvents.MouseMoveExt -= OnMouseMove;
            _timer.Enabled = false;
            _timer.Dispose();
        }
    }
}
