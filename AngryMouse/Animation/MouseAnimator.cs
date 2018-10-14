using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AngryMouse.Animation
{
    class MouseAnimator
    {
        /// <summary>
        /// Maximum cursor size.
        /// </summary>
        private const double MaxScale = 0.3;

        /// <summary>
        /// Scales the cursor.
        /// </summary>
        private readonly ScaleTransform _cursorScale;

        /// <summary>
        /// Whether the mouse is currently shaking or not.
        /// </summary>
        private bool _shaking;

        /// <summary>
        /// dpi info
        /// </summary>
        public DpiScale DpiInfo;

        /// <summary>
        /// For scale animation
        /// </summary>
        private readonly DoubleAnimation _scaleAnimation;

        public MouseAnimator(ScaleTransform cursorScale, DpiScale dpiInfo)
        {
            _cursorScale = cursorScale;
            DpiInfo = dpiInfo;

            Properties.Settings.Default.PropertyChanged += DefaultOnPropertyChanged;

            _scaleAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(Properties.Settings.Default.CursorAnimationLength)),
                EasingFunction = new CubicEase {EasingMode = EasingMode.EaseInOut},
                RepeatBehavior = new RepeatBehavior(1)
            };
        }

        private void DefaultOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("CursorAnimationLength"))
            {
                _scaleAnimation.Duration =
                    new Duration(TimeSpan.FromMilliseconds(Properties.Settings.Default.CursorAnimationLength));
            }
        }

        public void SetMouseShake(bool shaking, DateTime timestamp)
        {
            if (_shaking == shaking) return;
            _shaking = shaking;

            _scaleAnimation.From = _cursorScale.ScaleX;
            _scaleAnimation.To =
                shaking ? MaxScale * (Properties.Settings.Default.CursorSize / 10.0) * DpiInfo.PixelsPerDip : 0;

            _cursorScale.BeginAnimation(ScaleTransform.ScaleXProperty, _scaleAnimation);
            _cursorScale.BeginAnimation(ScaleTransform.ScaleYProperty, _scaleAnimation);
        }
    }
}
