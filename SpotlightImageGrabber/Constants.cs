namespace SpotlightImageGrabber
{
	public class Constants
	{

		public static string ASSETS_JSON { get; } = "assets.json";

		public static string APPLICATION_NAME { get; } = "SpotlightImageGrabber";

		public static string SPOTLIGHT { get; } = "Spotlight";

		public static string CONTENT_DELIVERY_SEARCH { get; } = "Microsoft.Windows.ContentDeliveryManager*";
		public static string LOCALSTATE_ASSETS { get; } = "LocalState\\Assets";

		public static string PACKAGES { get; } = "Packages";

		public static string LOG_LEVEL_TRACE { get; } = "Trace";
		public static string LOG_LEVEL_DEBUG { get; } = "Debug";
		public static string LOG_LEVEL_INFO { get; } = "Info";
		public static string LOG_LEVEL_WARNING { get; } = "Warning";
		public static string LOG_LEVEL_ERROR { get; } = "Error";
		public static string LOG_LEVEL_CRITICAL { get; } = "Critical";

		public static string STAT_SPOTLIGHT_ASSETS { get; } = "Spotlight Asset Count";
		public static string STAT_LOCAL_ASSETS { get; } = "Local Asset Count";
		public static string STAT_COPIED_ASSETS { get; } = "Copied Asset Count";

		public static string PROGRESS_INIT_CONFIG { get; } = "Initialized Configuration";
		public static string PROGRESS_INIT_FILESYSTEM { get; } = "Initialized Directories";
		public static string PROGRESS_LOADED_ASSETS { get; } = "Loaded Asset Data";
		public static string PROGRESS_LOADED_SPOTLIGHT { get; } = "Loaded Spotlight Data";
		public static string PROGRESS_COPIED_ASSETS { get; } = "Copied Assets";
		public static string PROGRESS_SAVED_ASSETS { get; } = "Saved Asset Data";

	}
}
