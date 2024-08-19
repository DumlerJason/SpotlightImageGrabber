using System.Runtime.CompilerServices;
using static SpotlightImageGrabber.Constants;

namespace SpotlightImageGrabber.Static
{
	public enum LogLevel
    {
        All = -1,
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Critical = 5
    }

    public class Log
    {

        static string DATESTAMP_FORMAT { get; } = "yyyy-MM-dd_HH-mm-ss.ff";
        static string DATE_FORMAT { get; } = "yyyy-MM-dd";
        static string PREFIX_TEMPLATE { get; } = "[{DateStamp} {LogLevel} {Source}]";

        public static bool LogToConsole { get; set; } = true;
        public static bool LogToFile { get; set; } = true;
        public static LogLevel LogLevel { get; private set; } = LogLevel.Info;

        static bool LogDirectory(out string log_directory, out string message, out Exception? exception)
        {
            log_directory = "";
            message = "";
            exception = null;

            try
            {
                string app_data_directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string application_app_data_directory = Path.Combine(app_data_directory, APPLICATION_NAME);
                log_directory = Path.Combine(application_app_data_directory, "logs");
                if (!Directory.Exists(log_directory)) Directory.CreateDirectory(log_directory);
                return true;
            }
            catch (Exception ex)
            {
                message = "An exception occurred.";
                exception = ex;
                return false;
            }
        }

        static bool LogFilePath(out string log_filepath, out string message, out Exception? exception)
        {
            message = "";
            exception = null;
            log_filepath = "";

            if (!LogDirectory(out string log_directory, out message, out exception)) return false;
            string filename = $"{DateTime.Now.ToString(DATE_FORMAT)}.log";
            log_filepath = Path.Combine(log_directory, filename);
            return true;
        }

        static bool FormatLogLinePrefix(LogLevel log_level, out string prefix, out string message, out Exception? exception, string caller_class = "", string caller_member_name = "", int caller_line_number = 0)
        {
            message = "";
            exception = null;
            prefix = "";

            string datestamp = DateTime.Now.ToString(DATESTAMP_FORMAT);
            string source = $"{caller_class}.{caller_member_name}:{caller_line_number}";

            prefix = PREFIX_TEMPLATE.Replace("{DateStamp}", datestamp).Replace("{LogLevel}", log_level.ToString()).Replace("{Source}", source);

            return true;
        }

        static bool FormatLines(LogLevel log_level, string log_message, out List<string> log_lines, out string message, out Exception? exception, string caller_class = "", string caller_member_name = "", int caller_line_number = 0)
        {
            log_lines = new List<string>();
            message = "";
            exception = null;

            if (!FormatLogLinePrefix(log_level, out string prefix, out message, out exception, caller_class, caller_member_name, caller_line_number))
            {
                return false;
            }

            string[] lines = log_message.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            foreach (string line in lines)
            {
                log_lines.Add($"{prefix} {line}");
            }

            return true;
        }

		public static void SetLogLevel(string log_level)
		{
			if (!Enum.TryParse(typeof(LogLevel), log_level.Trim(), true, out object? log_level_object))
			{
                LogLevel = LogLevel.Info;
			}
			else
			{
				LogLevel = (LogLevel)log_level_object;
			}
		}

		public static bool Write(LogLevel log_level, string log_message, out string message, out Exception? exception, [CallerFilePath] string caller_file_path = "", [CallerMemberName] string caller_member_name = "", [CallerLineNumber] int caller_line_number = 0)
        {
            message = "";
            exception = null;

            if (log_level < LogLevel) return true;

            if (!LogFilePath(out string logfile_path, out message, out exception)) return false;

            string caller_class = Path.GetFileNameWithoutExtension(caller_file_path);
            if (!FormatLines(log_level, log_message, out List<string> log_lines, out message, out exception, caller_class, caller_member_name, caller_line_number))
            {
                return false;
            }

            File.AppendAllLines(logfile_path, log_lines);

            if (LogToConsole)
            {
                foreach (string line in log_lines)
                {
                    Console.WriteLine(line);
                }
            }

            return true;
        }

        /// <summary>
        /// Write to the log at Info level, ignoring any messages or exceptions.
        /// </summary>
        /// <param name="log_message"></param>
        /// <param name="caller_file_path"></param>
        /// <param name="caller_member_name"></param>
        /// <param name="caller_line_number"></param>
        /// <returns></returns>
        public static bool Write(string log_message, [CallerFilePath] string caller_file_path = "", [CallerMemberName] string caller_member_name = "", [CallerLineNumber] int caller_line_number = 0)
        {
            return Write(LogLevel.Info, log_message, out string message, out Exception? exception, caller_file_path, caller_member_name, caller_line_number);
        }

        /// <summary>
        /// Write to the log ignoring any return messages or exceptions.
        /// </summary>
        /// <param name="log_level"></param>
        /// <param name="log_message"></param>
        /// <param name="caller_file_path"></param>
        /// <param name="caller_member_name"></param>
        /// <param name="caller_line_number"></param>
        /// <returns></returns>
        public static bool Write(LogLevel log_level, string log_message, [CallerFilePath] string caller_file_path = "", [CallerMemberName] string caller_member_name = "", [CallerLineNumber] int caller_line_number = 0)
        {
            return Write(log_level, log_message, out string message, out Exception? exception, caller_file_path, caller_member_name, caller_line_number);
        }

    }
}
