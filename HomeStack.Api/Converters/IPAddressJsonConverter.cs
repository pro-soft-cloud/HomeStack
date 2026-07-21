using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomeStack.Api.Converters;

public class IPAddressJsonConverter : JsonConverter<IPAddress>
{
	public override IPAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var value = reader.GetString();
		return value is null ? null : IPAddress.Parse(value);
	}

	public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString());
	}
}