using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizTools.Kahoot
{
    class ImageMetaData(string id, int width, int height, string contentType )
    {
        public string ID { get; } = id;
        public int Width { get; } = width;
        public int Height { get; } = height;
        public string ContentType { get; } = contentType;

        public string Link { get; } = $"https://media.kahoot.it/{id}";

        public static ImageMetaData FromJSON(JsonElement json)
        {
            if (json.Equals(default))
                return null;

            if (json.TryGetProperty("imageMetadata", out _))
                return FromJSON(json, "imageMetadata");
            if (json.TryGetProperty("image", out _))
                return FromJSON(json, "image");
            return null;
        }
        public static ImageMetaData FromJSON(JsonElement json, string name)
        {
            if (!json.TryGetProperty(name, out JsonElement element) || !element.TryGetProperty("id", out JsonElement id) || !element.TryGetProperty("width", out JsonElement width)
                || !element.TryGetProperty("height", out JsonElement height) || !element.TryGetProperty("contentType", out JsonElement contentType))
                return null;

            return new(id.GetString(), width.GetInt32(), height.GetInt32(), contentType.GetString());
        }

        public override string ToString() => $"{Link} --- {Width}x{Height} ({ContentType})";
    }
}
