using Gma.System.MouseKeyHook;

namespace AngryMouse.Mouse
{
    /// <summary>
    /// Keeps a static instance of the global events so the whole application uses a single instance.
    /// This also prevents local event objects from getting garbage collected and crashing the app.
    /// </summary>
    class StaticHook
    {
        private static IKeyboardMouseEvents _globalEvents;

        public static IKeyboardMouseEvents GlobalEvents()
        {
            return _globalEvents ?? (_globalEvents = Hook.GlobalEvents());
        }
    }
}
