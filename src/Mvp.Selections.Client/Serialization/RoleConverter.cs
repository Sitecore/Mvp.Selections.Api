using System.Text.Json;
using System.Text.Json.Serialization;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Client.Serialization
{
    /// <summary>
    /// JSON Converter for <see cref="Role"/>.
    /// </summary>
    /// <remarks>
    /// https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?pivots=dotnet-6-0#support-polymorphic-deserialization.
    /// </remarks>
    public class RoleConverter : JsonConverter<Role>
    {
        /// <summary>
        /// Read a <see cref="Role"/> from a <see cref="Utf8JsonReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
        /// <param name="typeToConvert">Target <see cref="Type"/> of <see cref="Role"/>.</param>
        /// <param name="options">Serialization options.</param>
        /// <returns>The deserialized <see cref="Role"/> or null.</returns>
        /// <exception cref="JsonException">When any unexpected tokens are encountered.</exception>
        public override Role? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Utf8JsonReader discriminatorReader = reader;
            if (discriminatorReader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Expected 'StartObject', seeing '{discriminatorReader.TokenType}' instead.");
            }

            discriminatorReader.Read();
            if (discriminatorReader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException($"Expected 'PropertyName', seeing '{discriminatorReader.TokenType}' instead.");
            }

            string? propertyName = discriminatorReader.GetString();
            if (propertyName != "$type")
            {
                throw new JsonException("Expected property '$type' first.");
            }

            discriminatorReader.Read();
            if (discriminatorReader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected property '$type' to be of type 'String', seeing '{discriminatorReader.TokenType}' instead.");
            }

            string typeDiscriminator = discriminatorReader.GetString() ?? string.Empty;
            Type? t = Type.GetType(typeDiscriminator);
            if (t == null || !t.IsAssignableTo(typeof(Role)))
            {
                throw new JsonException($"Type '{typeDiscriminator}' was invalid.");
            }

            return JsonSerializer.Deserialize(ref reader, t, options) as Role;
        }

        /// <summary>
        /// Write a <see cref="Role"/> to a <see cref="Utf8JsonWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> to use.</param>
        /// <param name="value">The <see cref="Role"/> to serialize.</param>
        /// <param name="options">The serialization options.</param>
        /// <exception cref="JsonException">When the type is incorrect.</exception>
        public override void Write(Utf8JsonWriter writer, Role value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            JsonConverter converter;
            Type roleType = value.GetType();
            writer.WriteString("$type", $"{roleType.FullName}, {roleType.Assembly.GetName().Name}");
            if (value is SystemRole systemRole)
            {
                writer.WriteString("rights", systemRole.Rights.ToString("F"));
            }
            else if (value is SelectionRole selectionRole)
            {
                converter = options.GetConverter(typeof(Application));
                if (selectionRole.Application != null && converter is JsonConverter<Application> applicationConverter)
                {
                    writer.WritePropertyName("application");
                    applicationConverter.Write(writer, selectionRole.Application, options);
                }
                else if (selectionRole.Application != null)
                {
                    throw new JsonException($"Converter '{converter}' is not of the right type.");
                }

                converter = options.GetConverter(typeof(Country));
                if (selectionRole.Country != null && converter is JsonConverter<Country> countryConverter)
                {
                    writer.WritePropertyName("country");
                    countryConverter.Write(writer, selectionRole.Country, options);
                }
                else if (selectionRole.Country != null)
                {
                    throw new JsonException($"Converter '{converter}' is not of the right type.");
                }

                converter = options.GetConverter(typeof(MvpType));
                if (selectionRole.MvpType != null && converter is JsonConverter<MvpType> mvpTypeConverter)
                {
                    writer.WritePropertyName("mvpType");
                    mvpTypeConverter.Write(writer, selectionRole.MvpType, options);
                }
                else if (selectionRole.MvpType != null)
                {
                    throw new JsonException($"Converter '{converter}' is not of the right type.");
                }

                converter = options.GetConverter(typeof(Region));
                if (selectionRole.Region != null && converter is JsonConverter<Region> regionConverter)
                {
                    writer.WritePropertyName("region");
                    regionConverter.Write(writer, selectionRole.Region, options);
                }
                else if (selectionRole.Region != null)
                {
                    throw new JsonException($"Converter '{converter}' is not of the right type.");
                }

                converter = options.GetConverter(typeof(Selection));
                if (selectionRole.Selection != null && converter is JsonConverter<Selection> selectionConverter)
                {
                    writer.WritePropertyName("selection");
                    selectionConverter.Write(writer, selectionRole.Selection, options);
                }
                else if (selectionRole.Selection != null)
                {
                    throw new JsonException($"Converter '{converter}' is not of the right type.");
                }
            }

            writer.WriteString("name", value.Name);
            writer.WriteString("id", value.Id.ToString("D"));
            writer.WriteString("createdOn", value.CreatedOn.ToString("s"));
            writer.WriteString("createdBy", value.CreatedBy);

            writer.WritePropertyName("users");
            converter = options.GetConverter(typeof(ICollection<User>));
            if (converter is JsonConverter<ICollection<User>> usersConverter)
            {
                usersConverter.Write(writer, value.Users, options);
            }
            else
            {
                throw new JsonException($"Converter '{converter}' is not of the right type.");
            }

            if (value.ModifiedOn != null)
            {
                writer.WriteString("modifiedOn", value.ModifiedOn.Value.ToString("s"));
            }

            if (!string.IsNullOrWhiteSpace(value.ModifiedBy))
            {
                writer.WriteString("modifiedBy", value.ModifiedBy);
            }

            writer.WriteEndObject();
        }
    }
}
