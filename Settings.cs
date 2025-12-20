using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

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


        public static string DirectoryPath => Path.GetDirectoryName(FilePath)!;
        public static string FilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RostMoment", "QuizTools", "Settings.json");

        public static void Save()
        {
            Directory.CreateDirectory(DirectoryPath);

            Dictionary<string, object> settingsDict = new Dictionary<string, object>();

            foreach (PropertyInfo property in typeof(Settings).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                SerializableSetting? attribute = property.GetCustomAttribute<SerializableSetting>();
                if (attribute != null)
                {
                    object value = property.GetValue(null)!;
                    settingsDict[attribute.Key] = value;
                }
            }

            string json = JsonSerializer.Serialize(settingsDict, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
        public static void Load()
        {
            if (!File.Exists(FilePath))
            {
                Save();
                Load();
                return;
            }

            string json = File.ReadAllText(FilePath);
            Dictionary<string, JsonElement> data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;

            foreach (PropertyInfo property in typeof(Settings).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                SerializableSetting? attribute = property.GetCustomAttribute<SerializableSetting>();
                if (attribute != null && data.TryGetValue(attribute.Key, out JsonElement value))
                    property.SetValue(null, value.Deserialize(property.PropertyType));
            }
            Save();
        }

        public static void OpenFolder()
        {
            Process.Start("explorer.exe", DirectoryPath);
        }
    }
}
