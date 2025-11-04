using Newtonsoft.Json;
using System;
using System.IO;
using QuizTools.GeneralUtils;

namespace QuizTools
{
    class Settings
    {
        public static string SettingsFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RostMoment",
            GeneralConstants.GUID,
            "Settings.json");

        private static Settings defaultSettings = new Settings();
        private static Settings _current;
        public static Settings Current => _current;

        [JsonProperty("min_delay")]
        public int MinDelay { get; private set; } = 2750;

        [JsonProperty("max_delay")]
        public int MaxDelay { get; private set; } = 3750;

        [JsonProperty("browser")]
        public Browser Browser { get; private set; } = Browser.Chrome;

        [JsonProperty("browser_binary_location")]
        public string BrowserBinaryLocation { get; private set; } = "";

        public static void Initialize()
        {
            _current = new Settings();
            _current.Load();
        }

        public void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath));
            File.WriteAllText(SettingsFilePath, JsonConvert.SerializeObject(Current));
        }

        public void Load()
        {
            if (!File.Exists(SettingsFilePath))
            {
                ResetToDefault(false);
                Save();
                return;
            }
            JsonConvert.PopulateObject(File.ReadAllText(SettingsFilePath), _current);
        }

        public void ResetToDefault() => ResetToDefault(true);
        public void ResetToDefault(bool needsConfirm)
        {
            if (needsConfirm)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Are you sure? Write 'yes' for confirm");
                Console.ResetColor();
                Console.Write("➤ ");
                if (Console.ReadLine().ToLower() != "yes")
                    return;
            }

            Current.MinDelay = defaultSettings.MinDelay;
            Current.MaxDelay = defaultSettings.MaxDelay;
            Current.Browser = defaultSettings.Browser;
            Current.BrowserBinaryLocation = defaultSettings.BrowserBinaryLocation;
            Logger.WriteInfoLine("Settings have been reset to default");
            Save();
        }
        public void ChangeDelay()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter min delay: ");
            Console.ResetColor();
            Console.Write("➤ ");
            if (!int.TryParse(Console.ReadLine(), out int min))
                return;


            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter max delay: ");
            Console.ResetColor();
            Console.Write("➤ ");
            if (!int.TryParse(Console.ReadLine(), out int max))
                return;

            if (max < min)
            {
                Logger.WriteErrorLine("You cannot set min delay bigger than max!");
                return;
            }
            if (max < 0 || min < 0)
            {
                Logger.WriteErrorLine("Delay cannot be negative");
                return;
            }
            Current.MinDelay = min;
            Current.MaxDelay = max;
            Save();
        }
        public void ChangeBrowser()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Select browser, enter its number");
            Console.ResetColor();
            Browser[] browsers = Enum.GetValues<Browser>();
            for (int i = 0; i < browsers.Length; i++)
                Console.WriteLine($"[{i + 1}] {browsers[i]}");

            Console.Write("➤ ");
            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0 || id > browsers.Length)
            {
                Logger.WriteErrorLine("Invalid input");
                return;
            }
            Current.Browser = browsers[id - 1];
            Save();
        }
    }
}
