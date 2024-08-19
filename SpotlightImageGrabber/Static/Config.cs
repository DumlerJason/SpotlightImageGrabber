using Newtonsoft.Json;

namespace SpotlightImageGrabber.Static
{


    /// <summary>
    /// Read a local config file.
    /// </summary>
    public class Config
    {
        public class ConfigDataSettings
        {
            public bool Enabled { get; set; } = false;

            public bool LogToConsole { get; set; } = true;

            public bool LogToFile { get; set; } = true;

            public string LogLevel { get; set; } = "";

            public ConfigDataSettings()
            {

            }

            public ConfigDataSettings(ConfigDataSettings copy_from)
            {
                Enabled = copy_from.Enabled;
                LogToConsole = copy_from.LogToConsole;
                LogToFile = copy_from.LogToFile;
                LogLevel = copy_from.LogLevel;
            }
        }

        static string ConfigFilePath { get; set; } = "";

        public static ConfigDataSettings ConfigData { get; set; } = new ConfigDataSettings();

        /// <summary>
        /// Read configuration data for the application.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static bool ReadConfigData(out string message, out Exception? exception)
        {
            message = "";
            exception = null;

            ConfigFilePath = ".\\Config.json";
            if (!File.Exists(ConfigFilePath))
            {
                string app_directory = AppDomain.CurrentDomain.BaseDirectory;

                // Combine the base directory with the relative path to the Config.json file
                ConfigFilePath = Path.Combine(app_directory, "Config.json");

                if (!File.Exists(ConfigFilePath))
                {
                    ConfigFilePath = "";
                    ConfigData = new ConfigDataSettings();
                    message = "Config file not found.";
                    return false;
                }
            }

            try
            {
                string json = File.ReadAllText(ConfigFilePath);
                ConfigDataSettings? buffer = JsonConvert.DeserializeObject<ConfigDataSettings>(json);
                if (buffer == null)
                {
                    message = "Invalid config data.";
                    ConfigData = new ConfigDataSettings();
                    return false;
                }

                ConfigData = new ConfigDataSettings(buffer);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                message = "An exception occurred.";
                return false;
            }
        }

    }
}
