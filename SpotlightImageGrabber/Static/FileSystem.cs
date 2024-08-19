using static SpotlightImageGrabber.Constants;


namespace SpotlightImageGrabber.Static
{
    public class FileSystem
    {

        /// <summary>
        /// Get the name of the Packages directory used for the Microsoft Spotlight system.
        /// </summary>
        /// <param name="packages_directory"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static bool GetPackagesDirectory(out string packages_directory, out string message, out Exception? exception)
        {
            message = "";
            exception = null;
            packages_directory = "";

            try
            {

                string app_data = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                packages_directory = Path.Combine(app_data, PACKAGES);

                if (Directory.Exists(packages_directory)) return true;

                message = $"Packages directory '{packages_directory}' not found.";
                return false;
            }
            catch (Exception ex)
            {
                message = "An exception occurred.";
                exception = ex;
                return false;
            }
        }

        public static bool GetAssetsDirectory(out string assets_directory, out string message, out Exception? exception)
        {
            message = "";
            exception = null;
            assets_directory = "";

            if (!GetPackagesDirectory(out string packages_directory, out message, out exception)) return false;
            string content_delivery_directory = Directory.GetDirectories(packages_directory, CONTENT_DELIVERY_SEARCH).ToList().FirstOrDefault() ?? "";
            if (content_delivery_directory == null || string.IsNullOrEmpty(content_delivery_directory))
            {
                message = $"Content Delivery Directory '{content_delivery_directory}' not found.";
                return false;
            }

            if (!Directory.Exists(content_delivery_directory))
            {
                message = $"Content Delivery Directory '{content_delivery_directory}' does not exist.";
                return false;
            }

            assets_directory = Path.Combine(content_delivery_directory, LOCALSTATE_ASSETS);
            if (!Directory.Exists(assets_directory))
            {
                message = $"Assets Directory '{assets_directory}' not found.";
                assets_directory = "";
                return false;
            }

            return true;
        }

        public static bool SetupTargetDirectory(out string target_directory, out string message, out Exception? exception)
        {
            message = "";
            exception = null;
            target_directory = "";
            try
            {

                string parent_directory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                if (!Directory.Exists(parent_directory))
                {
                    parent_directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    if (!Directory.Exists(parent_directory))
                    {
                        message = "Could not location user MyDocument or MyPictures directory.";
                        return false;
                    }
                }

                target_directory = Path.Combine(parent_directory, SPOTLIGHT);
                if (!Directory.Exists(target_directory)) Directory.CreateDirectory(target_directory);

                return true;
            }
            catch (Exception ex)
            {
                message = "An exception occurred.";
                exception = ex;
                return false;
            }
        }


    }
}
