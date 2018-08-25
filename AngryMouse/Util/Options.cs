using CommandLine;

namespace AngryMouse.Util
{
    /// <summary>
    /// This class defines the launch options for the app.
    /// </summary>
    class Options
    {
        /// <summary>
        /// If this is set the debug window is shown on launch.
        /// </summary>
        [Option('d', "debug", Default = false, HelpText = "Enable debug mode.", Required = false)]
        public bool Debug { get; set; }
    }
}
