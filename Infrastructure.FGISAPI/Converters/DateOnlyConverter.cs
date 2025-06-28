using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.FGISAPI.Converters;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        var dateString = reader.GetString() ?? throw new JsonException("Null value encountered for DateOnly");

        return DateOnly.Parse(dateString);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
