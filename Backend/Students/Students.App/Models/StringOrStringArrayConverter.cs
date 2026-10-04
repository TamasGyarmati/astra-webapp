using System.Text.Json;
using System.Text.Json.Serialization;

namespace Students.App.Models;

public class StringOrStringArrayConverter : JsonConverter<List<string>>
{
    public override bool HandleNull => true;

    public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                var single = reader.GetString();
                return string.IsNullOrWhiteSpace(single) ? new List<string>() : new List<string> { single };

            case JsonTokenType.StartArray:
                var list = new List<string>();

                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        var item = reader.GetString();

                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            list.Add(item);
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.Number)
                    {
                        list.Add(reader.GetDouble().ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                }

                return list;

            case JsonTokenType.Null:
                return new List<string>();

            default:
                throw new JsonException($"Unexpected token '{reader.TokenType}' while reading subject ids.");
        }
    }

    public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var item in value)
        {
            writer.WriteStringValue(item);
        }

        writer.WriteEndArray();
    }
}