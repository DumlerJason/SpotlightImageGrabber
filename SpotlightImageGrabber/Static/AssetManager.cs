using Newtonsoft.Json;
using System.Drawing;
using static SpotlightImageGrabber.Constants;


namespace SpotlightImageGrabber.Static
{
    public class AssetManager
    {

        public class AssetRecord
        {
            public string FilePath { get; set; } = "";
            public string FileChecksum { get; set; } = "";
            public string FileExtension { get; set; } = "";

            public int ImageWidth { get; set; } = 0;
            public int ImageHeight { get; set; } = 0;
        }


        public static bool GetSpotlightAssets(out List<AssetRecord> assets, out string message, out Exception? exception)
        {
            assets = new List<AssetRecord>();
            message = "";
            exception = null;

            if (!FileSystem.GetAssetsDirectory(out string assets_directory, out message, out exception)) return false;

            return GetAssetFiles(assets_directory, out assets, out message, out exception);
        }

        public static bool GetAssetFiles(string asset_directory, out List<AssetRecord> assets, out string message, out Exception? exception)
        {
            assets = new List<AssetRecord>();
            message = "";
            exception = null;

            if (!Directory.Exists(asset_directory))
            {
                message = "Target directory does not exist.";
                return false;
            }

            try
            {
                Log.Write(LogLevel.Debug, $"Checking {asset_directory} for assets.");

                List<string> asset_filelist = Directory.GetFiles(asset_directory).ToList();
                foreach (string asset_filepath in asset_filelist)
                {
                    string md5 = MD5Helper.GetMd5Checksum(asset_filepath);
                    if (assets.Count(f => f.FileChecksum == md5) == 0 && ImageHelper.IsImageFile(asset_filepath, out string extension))
                    {
                        // Get the image's dimensions so we avoid really small ones.
                        Size s = ImageHelper.ImageDimensions(asset_filepath);
                        if (s.Width < 720 || s.Height < 480) continue;

                        assets.Add(new AssetRecord()
                        {
                            FileChecksum = md5,
                            FilePath = asset_filepath,
                            ImageHeight = s.Height,
                            ImageWidth = s.Width,
                            FileExtension = extension
                        });
                    }
                    else
                    {
                        Log.Write(LogLevel.Trace, $"{asset_filepath} already in assets or it not an image file.");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                message = "An exception occurred.";
                exception = ex;
                return false;
            }
        }

        public static bool ReconcileAssetFiles(string asset_directory, ref List<AssetRecord> assets, out string message, out Exception? exception)
        {
            message = "";
            exception = null;

            if (!Directory.Exists(asset_directory))
            {
                message = "Asset directory does not exist.";
                return false;
            }

            try
            {
                if (!GetAssetFiles(asset_directory, out List<AssetRecord> actual_assets, out message, out exception))
                {
                    message = $"Unable to retrieve assets; {message}";
                    return false;
                }

                List<AssetRecord> invalid_assets = new List<AssetRecord>();

                foreach (AssetRecord possible_asset in assets)
                {
                    // See if we have invalid records.
                    if (actual_assets.Count(a => a.FileChecksum == possible_asset.FileChecksum) == 0)
                    {
                        Log.Write(LogLevel.Debug, $"{Path.GetFileName(possible_asset.FilePath)} invalid, removing.");
                        invalid_assets.Add(possible_asset);
                    }
                }

                // Remove the invalid asset records.
                foreach (AssetRecord invalid_asset in invalid_assets)
                {
                    assets.RemoveAll(a => a.FileChecksum == invalid_asset.FileChecksum);
                }

                foreach (AssetRecord known_asset in actual_assets)
                {
                    // See if we're missing asset records.
                    if (assets.Count(a => a.FileChecksum == known_asset.FileChecksum) == 0)
                    {
                        Log.Write(LogLevel.Debug, $"{Path.GetFileName(known_asset.FilePath)} being added to assets.");
                        assets.Add(known_asset);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                message = "An exception occurred.";
                exception = ex;
                return false;
            }
        }

        public static bool SaveAssetsData(List<AssetRecord> assets, out string message, out Exception? exception)
        {
            message = "";
            exception = null;

            try
            {
                string app_data_directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string application_app_data_directory = Path.Combine(app_data_directory, APPLICATION_NAME);
                if (!Directory.Exists(application_app_data_directory)) Directory.CreateDirectory(application_app_data_directory);
                string assets_filepath = Path.Combine(application_app_data_directory, ASSETS_JSON);
                string json = JsonConvert.SerializeObject(assets, Formatting.Indented);
                File.WriteAllText(assets_filepath, json);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                message = "An exception occurred.";
                return false;
            }
        }

        public static bool LoadAssetData(out List<AssetRecord> assets, out string message, out Exception? exception)
        {
            message = "";
            exception = null;
            assets = new List<AssetRecord>();

            try
            {
                string app_data_directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string application_app_data_directory = Path.Combine(app_data_directory, APPLICATION_NAME);
                if (!Directory.Exists(application_app_data_directory))
                {
                    // Create the directory while we are here.
                    Directory.CreateDirectory(application_app_data_directory);
                    message = "Assets file not initialized.";
                    return false;
                }

                string assets_filepath = Path.Combine(application_app_data_directory, ASSETS_JSON);

                if (!File.Exists(assets_filepath))
                {
                    message = "Assets file not initialized.";
                    return false;
                }

                string json = JsonConvert.SerializeObject(assets, Formatting.Indented);
                File.WriteAllText(assets_filepath, json);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                message = "An exception occurred.";
                return false;
            }
        }

        public static bool CopyAssetFiles(string target_directory, List<AssetRecord> spotlight_assets, ref List<AssetRecord> current_assets, out string message, out Exception? exception)
        {
            message = "";
            exception = null;

            try
            {
                foreach (AssetRecord spotlight_asset in spotlight_assets)
                {
                    if (current_assets.Count(a => a.FileChecksum == spotlight_asset.FileChecksum) == 0)
                    {

                        string source_filename = spotlight_asset.FilePath;
                        string target_filename = Path.Combine(target_directory, $"{Path.GetFileNameWithoutExtension(spotlight_asset.FilePath)}.{spotlight_asset.FileExtension}");

                        Log.Write(LogLevel.Debug, $"Copying new asset {Path.GetFileName(spotlight_asset.FilePath)}\r\n -- to {target_filename}");

                        File.Copy(source_filename, target_filename);

                        if (File.Exists(target_filename))
                        {
                            current_assets.Add(new AssetRecord()
                            {
                                FilePath = target_filename,
                                FileChecksum = spotlight_asset.FileChecksum,
                                FileExtension = spotlight_asset.FileExtension,
                                ImageHeight = spotlight_asset.ImageHeight,
                                ImageWidth = spotlight_asset.ImageWidth
                            });
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                message = "An exception has occurred.";
                return false;
            }
        }



    }
}
