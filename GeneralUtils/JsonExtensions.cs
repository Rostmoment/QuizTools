using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace QuizTools.GeneralUtils
{
    public static class JsonExtensions
    {
        public static JsonElement ChangeValue<T>(this JsonElement json, string key, T newValue)
        {
            if (json.ValueKind != JsonValueKind.Object)
                throw new ArgumentException("JsonElement must be an object.", nameof(json));

            JsonObject? obj = JsonNode.Parse(json.GetRawText())?.AsObject();
            if (obj is null)
                throw new InvalidOperationException("Failed to parse JsonElement to JsonObject.");

            if (!obj.ContainsKey(key))
                throw new KeyNotFoundException($"Key '{key}' not found in JSON object.");

            obj[key] = JsonValue.Create(newValue);

            return JsonSerializer.SerializeToElement(obj);
        }
        public static bool GetBooleanOrDefault(this JsonElement json, string key, bool defaultValue = false)
        {
            if (!json.TryGetProperty(key, out JsonElement element)) 
                return defaultValue;
            try
            {
                return element.GetBoolean();
            }
            catch
            {
                return defaultValue;
            }
        }
        public static double GetDoubleOrDefault(this JsonElement json, string key, double defaultValue = double.NaN)
        {
            if (!json.TryGetProperty(key, out JsonElement element))
                return defaultValue;
            try
            {
                return element.GetDouble();
            }
            catch
            {
                return defaultValue;
            }
        }
        public static int GetInt32OrDefault(this JsonElement json, string key, int defaultValue = 0)
        {
            if (!json.TryGetProperty(key, out JsonElement element))
                return defaultValue;
            try
            {
                return element.GetInt32();
            }
            catch
            {
                return defaultValue;
            }
        }
        public static string GetStringOrDefault(this JsonElement json, string key, string defaultValue = null)
        {
            if (!json.TryGetProperty(key, out JsonElement element))
                return defaultValue;
            try
            {
                return element.GetString();
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
