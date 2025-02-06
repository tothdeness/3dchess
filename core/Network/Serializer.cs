using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace test.core.Network
{
	public static class Serializer // Still static
	{
		private static JsonSerializerOptions _options;

		static Serializer()
		{
			_options = new JsonSerializerOptions();
			_options.Converters.Add(new GodotVector3JsonConverter());
			// Other options if needed (e.g., _options.WriteIndented = true;)
		}

		public static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, _options);
		public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _options);


		// GodotVector3JsonConverter is now *nested* inside the Serializer class
		public class GodotVector3JsonConverter : JsonConverter<Vector3>
		{
			public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				if (reader.TokenType != JsonTokenType.StartObject)
				{
					throw new JsonException();
				}

				float x = 0, y = 0, z = 0;

				while (reader.Read())
				{
					if (reader.TokenType == JsonTokenType.EndObject)
					{
						return new Vector3(x, y, z);
					}

					if (reader.TokenType == JsonTokenType.PropertyName)
					{
						string propertyName = reader.GetString();
						reader.Read();

						switch (propertyName)
						{
							case "x": x = reader.GetSingle(); break;
							case "y": y = reader.GetSingle(); break;
							case "z": z = reader.GetSingle(); break;
							default: throw new JsonException($"Unexpected property: {propertyName}");
						}
					}
				}

				throw new JsonException();
			}

			public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
			{
				writer.WriteStartObject();
				writer.WriteNumber("x", value.X);
				writer.WriteNumber("y", value.Y);
				writer.WriteNumber("z", value.Z);
				writer.WriteEndObject();
			}
		}
	}
}