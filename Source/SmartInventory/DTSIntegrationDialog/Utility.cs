using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTSIntegrationDialog
{
    public class CustomDateTimeConverter : JsonConverter<DateTime?>
    {
        public override void WriteJson(JsonWriter writer, DateTime? value, JsonSerializer serializer)
        {
            if (value.HasValue)
            {
                writer.WriteValue(value.Value.ToString("o")); // Use the desired date format
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override DateTime? ReadJson(JsonReader reader, Type objectType, DateTime? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var dateStr = (string)reader.Value;
                if (DateTime.TryParse(dateStr, out DateTime date))
                {
                    return date;
                }
                if (dateStr == "0")
                {
                    return null;
                }
            }
            return null;
        }
    }

}
