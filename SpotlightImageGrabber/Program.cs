using SpotlightImageGrabber.Static;
using static SpotlightImageGrabber.Static.AssetManager;
using static SpotlightImageGrabber.Constants;
using Newtonsoft.Json;

string message = "";
Exception? exception = null;
Dictionary<string, int> statistics = new Dictionary<string, int>()
{
	{ STAT_SPOTLIGHT_ASSETS, 0 },
	{ STAT_LOCAL_ASSETS, 0 },
	{ STAT_COPIED_ASSETS, 0 },
	{ PROGRESS_INIT_CONFIG, 0 },
	{ PROGRESS_INIT_FILESYSTEM, 0 },
	{ PROGRESS_LOADED_ASSETS, 0 },
	{ PROGRESS_LOADED_SPOTLIGHT, 0 },
	{ PROGRESS_COPIED_ASSETS, 0 },
	{ PROGRESS_SAVED_ASSETS, 0 },
};

if (!Config.ReadConfigData(out message, out exception))
{
	Log.Write(LogLevel.Error, $"Initialization failed, could not read config file.");
	if (!string.IsNullOrEmpty(message)) Log.Write(LogLevel.Error, $"Message: {message}");
	if (exception == null) Log.Write(LogLevel.Error, $"Exception: {exception}");
	return;
}

statistics[PROGRESS_INIT_CONFIG] = 1;
Log.LogToConsole = Config.ConfigData.LogToConsole;
Log.LogToFile = Config.ConfigData.LogToFile;
Log.SetLogLevel(Config.ConfigData.LogLevel);
Log.Write(LogLevel.Info, $"SpotlightImageGrabber initializing.", out message, out exception);

if (!FileSystem.SetupTargetDirectory(out string target_directory, out message, out exception))
{
	Log.Write(LogLevel.Error, $"Initialization failed, could not setup target directory.");
	if (!string.IsNullOrEmpty(message)) Log.Write(LogLevel.Error, $"Message: {message}");
	if (exception == null) Log.Write(LogLevel.Error, $"Exception: {exception}");
	return;
}

Log.Write($"Reading current asset files.");
statistics[PROGRESS_INIT_FILESYSTEM] = 1;

if (!AssetManager.LoadAssetData(out List<AssetRecord> current_assets, out message, out exception))
{
	Log.Write($"Could not load asset data, refreshing asset data.");

	// Something went wrong with getting the current list of assets.
	if (!AssetManager.GetAssetFiles(target_directory, out current_assets, out message, out exception))
	{
		Log.Write(LogLevel.Error, $"Unable to get a reference to the current asset files.");
		if (!string.IsNullOrEmpty(message)) Log.Write(LogLevel.Error, $"Message: {message}");
		if (exception == null) Log.Write(LogLevel.Error, $"Exception: {exception}");
		return;
	}
	else
	{
		Log.Write($"Assets data initialized, saving.");

		if (!AssetManager.SaveAssetsData(current_assets, out message, out exception))
		{
			Log.Write(LogLevel.Error, $"Could not save current assets data.");
			if (!string.IsNullOrEmpty(message)) Log.Write(LogLevel.Error, $"Message: {message}");
			if (exception == null) Log.Write(LogLevel.Error, $"Exception: {exception}");
			return;
		}
	}
}
else
{
	// Need to reconcile current assets with the list of assets.
	if (!AssetManager.ReconcileAssetFiles(target_directory, ref current_assets, out message, out exception))
	{
		Log.Write(LogLevel.Error, $"Unable to reconcile assets.");
		if (!string.IsNullOrEmpty(message)) Log.Write(LogLevel.Error, $"Message: {message}");
		if (exception == null) Log.Write(LogLevel.Error, $"Exception: {exception}");
		return;
	}
}

statistics[STAT_LOCAL_ASSETS] = current_assets.Count;
statistics[PROGRESS_LOADED_ASSETS] = 1;

Log.Write($"Current assets ({current_assets.Count}) read and reconciled if necessary.");

Log.Write($"Reading Spotlight assets.");

if (!AssetManager.GetSpotlightAssets(out List<AssetRecord> spotlight_assets, out message, out exception))
{
	Log.Write(LogLevel.Error, $"Unable to get a reference to the Spotlight asset files.");
	if (!string.IsNullOrEmpty(message)) Log.Write(LogLevel.Error, $"Message: {message}");
	if (exception == null) Log.Write(LogLevel.Error, $"Exception: {exception}");
	return;
}

statistics[STAT_SPOTLIGHT_ASSETS] = spotlight_assets.Count;
statistics[PROGRESS_LOADED_SPOTLIGHT] = 1;

Log.Write($"Copying new Spotlight assets.");

if (!AssetManager.CopyAssetFiles(target_directory, spotlight_assets, ref current_assets, out message, out exception))
{
	Log.Write(LogLevel.Error, $"An exception occurred while copying asset files.");
	if (!string.IsNullOrEmpty(message)) Log.Write(LogLevel.Error, $"Message: {message}");
	if (exception == null) Log.Write(LogLevel.Error, $"Exception: {exception}");
	return;
}

statistics[STAT_COPIED_ASSETS] = current_assets.Count - statistics["Local Asset Count"];
statistics[PROGRESS_COPIED_ASSETS] = 1;

Log.Write($"Saving assets data.");

if (!AssetManager.SaveAssetsData(current_assets, out message, out exception))
{
	Log.Write(LogLevel.Error, $"An error occurred saving the current assets data.");
	if (!string.IsNullOrEmpty(message)) Log.Write(LogLevel.Error, $"Message: {message}");
	if (exception == null) Log.Write(LogLevel.Error, $"Exception: {exception}");
	statistics[PROGRESS_SAVED_ASSETS] = 0;
}
else
{
	statistics[PROGRESS_SAVED_ASSETS] = 1;
}

Log.Write(JsonConvert.SerializeObject(statistics, Formatting.Indented));
