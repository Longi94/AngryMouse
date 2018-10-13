using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AngryMouse.Animation
{
    class MouseAnimator
    {
        /// <summary>
        /// Time of the animation if the scaling goes from 0 to Max or vice versa.
        /// </summary>
        private const int MaxAnimationLength = 200;

        /// <summary>
        /// Maximum cursor size.
        /// </summary>
        private const double MaxScale = 0.3;

        /// <summary>
        /// Scales the cursor.
        /// </summary>
        private readonly ScaleTransform _cursorScale;

        /// <summary>
        /// Timer that does the animation
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Wether the mouse is currently shaking or not.
        /// </summary>
        private bool _shaking;

        /// <summary>
        /// Is the mouse inside the current screen.
        /// </summary>
        public bool MouseInScreen;

        /// <summary>
        /// The current scale of the cursor. Stored so we can start a new animation from the middle scale if needed.
        /// </summary>
        private double _currentScale;

        /// <summary>
        /// The start of the animation
        /// </summary>
        private DateTime _animationStart = DateTime.MinValue;

        /// <summary>
        /// The scale to start the animation from.
        /// </summary>
        private double _scaleAnimStart;

        /// <summary>
        /// The scale to end the animation at.
        /// </summary>
        private double _scaleAnimEnd = MaxScale;

        /// <summary>
        /// The canvas element that should be scaled, hidden and shown.
        /// </summary>
        private readonly Path _bigCursor;

        /// <summary>
        /// dpi info
        /// </summary>
        public DpiScale DpiInfo;

        public MouseAnimator(ScaleTransform cursorScale, Path bigCursor, DpiScale dpiInfo)
        {
            _cursorScale = cursorScale;
            _bigCursor = bigCursor;
            DpiInfo = dpiInfo;
        }

        public void SetMouseShake(bool shaking, DateTime timestamp)
        {
            if (_shaking == shaking) return;
            _shaking = shaking;

            _animationStart = timestamp;

            _scaleAnimStart = _currentScale;
            _scaleAnimEnd = shaking ? MaxScale : 0;

            StartTimer();
        }

        private void StartTimer()
        {
            StopTimer();
            _timer = new Timer();
            _timer.Elapsed += Timer_Tick;
            _timer.Interval = 33;
            _timer.Enabled = true;
        }

        private void StopTimer()
        {
            if (_timer == null) return;
            _timer.Enabled = false;
            _timer.Dispose();
            _timer = null;
        }

        private void Timer_Tick(object sender, ElapsedEventArgs e)
        {
            if (!MouseInScreen)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _cursorScale.ScaleX = _cursorScale.ScaleY = 0;
                    _bigCursor.Visibility = Visibility.Hidden;
                });
                return;
            }

            if ((e.SignalTime - _animationStart).TotalMilliseconds > MaxAnimationLength)
            {
                StopTimer();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (_shaking)
                    {
                        _cursorScale.ScaleX = _cursorScale.ScaleY = MaxScale * (Properties.Settings.Default.CursorSize / 10.0) * DpiInfo.PixelsPerDip;
                        _bigCursor.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _cursorScale.ScaleX = _cursorScale.ScaleY = 0;
                        _bigCursor.Visibility = Visibility.Hidden;
                    }
                });
            }
            else
            {
                var scale = CalculateScale(e.SignalTime) * (Properties.Settings.Default.CursorSize / 10.0);

                if (scale < 0 || scale > MaxScale)
                {
                    return;
                }

                _currentScale = scale * DpiInfo.PixelsPerDip;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _cursorScale.ScaleX = _cursorScale.ScaleY = _currentScale;
                    _bigCursor.Visibility = Visibility.Visible;
                });
            }
        }

        /// <summary>
        /// Calculate the current scale of the cursor.
        /// </summary>
        /// <param name="timestamp">The timestamp of the frame the calculate for.</param>
        /// <returns></returns>
        private double CalculateScale(DateTime timestamp)
        {
            var animLength = Math.Abs(_scaleAnimEnd - _scaleAnimStart) / MaxScale * MaxAnimationLength;

            var elapsed = (timestamp - _animationStart).TotalMilliseconds;

            var t = elapsed / animLength;

            var tEase = Easing.CubicInOut(t);

            return (_scaleAnimEnd - _scaleAnimStart) * tEase + _scaleAnimStart;
        }
    }
}
