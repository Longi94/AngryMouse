using System.Windows;

namespace AngryMouse
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the window is successfully loaded. Does some view initialization.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SizeSlider.Value = Properties.Settings.Default.CursorSize;
            SizeSlider.ValueChanged += SizeSlider_OnValueChanged;
            AnimationLengthSlider.Value = Properties.Settings.Default.CursorAnimationLength;
            AnimationLengthSlider.ValueChanged += AnimationLengthSlider_OnValueChanged;
            AngryCheckBox.IsChecked = Properties.Settings.Default.Angry;
            AngryCheckBox.Checked += (o, args) => {
                AngryCheckBox_OnValueChanged(true);
            };
            AngryCheckBox.Unchecked += (o, args) => {
                AngryCheckBox_OnValueChanged(false);
            };
        }

        private void AngryCheckBox_OnValueChanged(bool value) {
            Properties.Settings.Default.Angry = value;
            Properties.Settings.Default.Save();
        }
        
        
        private void SizeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Properties.Settings.Default.CursorSize = e.NewValue;
            Properties.Settings.Default.Save();
        }

        private void AnimationLengthSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Properties.Settings.Default.CursorAnimationLength = (int) e.NewValue;
            Properties.Settings.Default.Save();
        }
    }
}
