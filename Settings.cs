using QuizTools.GeneralUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuizTools
{
    public class Settings
    {

        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
        private class SerializableSetting : Attribute
        {
            public string Key { get; }
            public SerializableSetting(string key)
            {
                Key = key;
            }
        }

        private static Settings Default { get; set; }
        public static Settings Instance { get; private set; }

        public static string DirectoryPath => Path.GetDirectoryName(FilePath)!;
        public static string FilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RostMoment", "QuizTools", "Settings.json");

        [SerializableSetting("LogoGradientFrom")]
        public string LogoGradientFrom { get; set; } = "#710303";
        [SerializableSetting("LogoGradientTo")]
        public string LogoGradientTo { get; set; } = "#3c0346";

        public static void Initialize()
        {
            Default = new Settings();
            Instance = new Settings();
            Instance.Load();
        }

        public void Save()
        {
            Directory.CreateDirectory(DirectoryPath);

            Dictionary<string, object> settingsDict = new Dictionary<string, object>();

            foreach (PropertyInfo property in typeof(Settings).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                SerializableSetting? attribute = property.GetCustomAttribute<SerializableSetting>();
                if (attribute != null)
                {
                    object value = property.GetValue(Instance)!;
                    settingsDict[attribute.Key] = value;
                }
            }

            string json = JsonSerializer.Serialize(settingsDict, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
        public void Load()
        {
            if (!File.Exists(FilePath))
            {
                Save();
                Load();
                return;
            }

            string json = File.ReadAllText(FilePath);
            Dictionary<string, JsonElement> data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;

            foreach (PropertyInfo property in typeof(Settings).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                SerializableSetting? attribute = property.GetCustomAttribute<SerializableSetting>();
                if (attribute != null && data.TryGetValue(attribute.Key, out JsonElement value))
                    property.SetValue(Instance, value.Deserialize(property.PropertyType));
            }
            Save();
        }


        public static void SetLogoGradient()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter the starting hex color for logo gradient");
            Console.ResetColor();
            ConsoleUtils.WriteArrow();

            string from = Console.ReadLine()!;
            if (!ColorHelper.IsValidHex(from))
            {
                Logger.WriteErrorLine("Invalid Hex Color");
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter the ending hex color for logo gradient");
            Console.ResetColor();
            ConsoleUtils.WriteArrow();
            string to = Console.ReadLine()!;
            if (!ColorHelper.IsValidHex(to))
            {
                Logger.WriteErrorLine("Invalid Hex Color");
                return;
            }
            Instance.LogoGradientFrom = from;
            Instance.LogoGradientTo = to;
            Instance.Save();
        }
        public static void Reset()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Are you sure? Write 'yes' if so");
            Console.ResetColor();
            ConsoleUtils.WriteArrow();
            if (Console.ReadLine()!.ToLower() != "yes")
                return;

            foreach (PropertyInfo property in typeof(Settings).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                SerializableSetting? attribute = property.GetCustomAttribute<SerializableSetting>();
                if (attribute != null)
                {
                    object value = property.GetValue(Default)!;
                    property.SetValue(Instance, value);
                }
            }
            Logger.WriteInfoLine("All settings were reset to default values");
            Instance.Save();
        }
        public static void OpenFolder()
        {
            Process.Start("explorer.exe", DirectoryPath);
        }
    }
}
      