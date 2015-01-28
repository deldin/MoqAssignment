using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;

namespace Ista.FileServices.Infrastructure.Serializers
{
    public class JsonMessageSerializer
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
            TypeNameHandling = TypeNameHandling.None,
        };

        public static T DeserializeType<T>(byte[] bytes, T type)
        {
            var content = Encoding.UTF8.GetString(bytes);
            return DeserializeType(content, type);
        }

        public static T DeserializeType<T>(string content, T type)
        {
            return JsonConvert.DeserializeAnonymousType(content, type);
        }

        public static T DeserializeType<T>(byte[] bytes)
        {
            var content = Encoding.UTF8.GetString(bytes);
            return DeserializeType<T>(content);
        }

        public static T DeserializeType<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content, settings);
        }

        public static byte[] SerializeToBytes<T>(T message)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                var serializer = JsonSerializer.Create(settings);
                serializer.Serialize(jsonWriter, message);

                writer.Flush();
                return stream.ToArray();
            }
        }

        public static string SerializeToString<T>(T message)
        {
            using (var writer = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                var serializer = JsonSerializer.Create(settings);
                serializer.Serialize(jsonWriter, message);

                writer.Flush();
                return writer.ToString();
            }
        }
    }
}
