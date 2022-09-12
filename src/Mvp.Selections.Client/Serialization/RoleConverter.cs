using System.Text.Json;
using System.Text.Json.Serialization;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Client.Serialization
{
    // NOTE [ILs] https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?pivots=dotnet-6-0#support-polymorphic-deserialization
    public class RoleConverter : JsonConverter<Role>
    {
        public override Role Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Expected 'StartObject', seeing '{reader.TokenType}' instead.");
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException($"Expected 'PropertyName', seeing '{reader.TokenType}' instead.");
            }

            string? propertyName = reader.GetString();
            if (propertyName != "$type")
            {
                throw new JsonException("Expected property '$type' first.");
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected property '$type' to be of type 'String', seeing '{reader.TokenType}' instead.");
            }

            string typeDiscriminator = reader.GetString() ?? string.Empty;
            Role result = GetSubClass(typeDiscriminator, Guid.Empty);

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                    reader.Read();
                    JsonConverter converter;
                    switch (propertyName)
                    {
                        // SystemRole
                        case "rights":
                            string rights = reader.GetString() ?? string.Empty;
                            if (Enum.TryParse(typeof(Right), rights, false, out object? rightsResult) && rightsResult != null)
                            {
                                ((SystemRole)result).Rights = (Right)rightsResult;
                            }

                            break;

                        // SelectionRole
                        case "country":
                            converter = options.GetConverter(typeof(Country));
                            if (converter is JsonConverter<Country> countryConverter)
                            {
                                ((SelectionRole)result).Country = countryConverter.Read(ref reader, typeof(Country), options);
                            }

                            break;
                        case "application":
                            converter = options.GetConverter(typeof(Application));
                            if (converter is JsonConverter<Application> applicationConverter)
                            {
                                ((SelectionRole)result).Application = applicationConverter.Read(ref reader, typeof(Application), options);
                            }

                            break;
                        case "mvpType":
                            converter = options.GetConverter(typeof(MvpType));
                            if (converter is JsonConverter<MvpType> mvpTypeConverter)
                            {
                                ((SelectionRole)result).MvpType = mvpTypeConverter.Read(ref reader, typeof(MvpType), options);
                            }

                            break;
                        case "region":
                            converter = options.GetConverter(typeof(Region));
                            if (converter is JsonConverter<Region> regionConverter)
                            {
                                ((SelectionRole)result).Region = regionConverter.Read(ref reader, typeof(Region), options);
                            }

                            break;
                        case "selection":
                            converter = options.GetConverter(typeof(Selection));
                            if (converter is JsonConverter<Selection> selectionConverter)
                            {
                                ((SelectionRole)result).Selection = selectionConverter.Read(ref reader, typeof(Selection), options);
                            }

                            break;

                        // Role
                        case "name":
                            result.Name = reader.GetString() ?? string.Empty;
                            break;

                        // BaseEntity
                        case "id":
                            Role roleWithId = GetSubClass(typeDiscriminator, reader.GetGuid());
                            roleWithId.Name = result.Name;
                            roleWithId.CreatedBy = result.CreatedBy;
                            roleWithId.CreatedOn = result.CreatedOn;
                            roleWithId.ModifiedBy = result.ModifiedBy;
                            roleWithId.ModifiedOn = result.ModifiedOn;
                            if (roleWithId is SystemRole systemRole)
                            {
                                systemRole.Rights = ((SystemRole)result).Rights;
                            }
                            else if (roleWithId is SelectionRole selectionRole)
                            {
                                selectionRole.Country = ((SelectionRole)result).Country;
                                selectionRole.Application = ((SelectionRole)result).Application;
                                selectionRole.MvpType = ((SelectionRole)result).MvpType;
                                selectionRole.Region = ((SelectionRole)result).Region;
                                selectionRole.Selection = ((SelectionRole)result).Selection;
                            }

                            result = roleWithId;
                            break;
                        case "createdOn":
                            result.CreatedOn = reader.GetDateTime();
                            break;
                        case "createdBy":
                            result.CreatedBy = reader.GetString() ?? string.Empty;
                            break;
                        case "modifiedOn":
                            result.ModifiedOn = reader.GetDateTime();
                            break;
                        case "modifiedBy":
                            result.ModifiedBy = reader.GetString() ?? string.Empty;
                            break;
                    }
                }
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Role value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            Type roleType = value.GetType();
            writer.WriteString("$type", $"{roleType.FullName}, {roleType.Assembly.GetName().Name}");
            if (value is SystemRole systemRole)
            {
                writer.WriteString("rights", systemRole.Rights.ToString("F"));
            }
            else if (value is SelectionRole selectionRole)
            {
                JsonConverter converter = options.GetConverter(typeof(Country));
                if (selectionRole.Country != null && converter is JsonConverter<Country> countryConverter)
                {
                    writer.WritePropertyName("country");
                    countryConverter.Write(writer, selectionRole.Country, options);
                }
            }

            writer.WriteString("name", value.Name);
            writer.WriteString("id", value.Id.ToString("D"));
            writer.WriteString("createdOn", value.CreatedOn.ToString("s"));
            writer.WriteString("createdBy", value.CreatedBy);

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

        private static Role GetSubClass(string typeDiscriminator, Guid id)
        {
            Role result = typeDiscriminator switch
            {
                "Mvp.Selections.Domain.SystemRole, Mvp.Selections.Domain" => new SystemRole(id),
                "Mvp.Selections.Domain.SelectionRole, Mvp.Selections.Domain" => new SelectionRole(id),
                _ => throw new JsonException($"SubType '{typeDiscriminator}' is unsupported.")
            };

            return result;
        }
    }
}
