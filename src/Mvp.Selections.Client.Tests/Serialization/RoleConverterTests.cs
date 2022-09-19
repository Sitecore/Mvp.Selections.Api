using System.Text.Json;
using Mvp.Selections.Client.Serialization;
using System.Text.Json.Serialization;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Client.Tests.Serialization
{
    public class RoleConverterTests
    {
        [Fact]
        public void CanSerializeSystemRole()
        {
            // Arrange
            Role role = new SystemRole(Guid.NewGuid())
            {
                Name = "Test",
                CreatedOn = DateTime.Today.AddDays(-1),
                CreatedBy = "System",
                ModifiedOn = DateTime.Today,
                ModifiedBy = "Test",
                Rights = Right.Apply | Right.Review
            };
            using MemoryStream ms = new ();

            // Act
            JsonSerializer.Serialize(ms, role, GetOptions());

            // Assert
            ms.Flush();
            ms.Position = 0;
            using TextReader reader = new StreamReader(ms);
            string json = reader.ReadToEnd();

            Assert.Matches(".*\"\\$type\":\"Mvp.Selections.Domain.SystemRole, Mvp.Selections.Domain\".*", json);
            Assert.Matches(".*\"rights\":\"Apply, Review\".*", json);
        }

        [Fact]
        public void CanSerializeSelectionRole()
        {
            // Arrange
            Role role = new SelectionRole(Guid.NewGuid())
            {
                Name = "Test",
                CreatedOn = DateTime.Today.AddDays(-1),
                CreatedBy = "System",
                ModifiedOn = DateTime.Today,
                ModifiedBy = "Test",
                Country = new Country(1)
                {
                    Name = "Country A",
                    CreatedOn = DateTime.Today.AddDays(-1),
                    CreatedBy = "System",
                    Region = new Region(1)
                    {
                        Name = "Region A",
                        CreatedOn = DateTime.Today.AddDays(-1),
                        CreatedBy = "System"
                    }
                }
            };
            using MemoryStream ms = new ();

            // Act
            JsonSerializer.Serialize(ms, role, GetOptions());

            // Assert
            ms.Flush();
            ms.Position = 0;
            using TextReader reader = new StreamReader(ms);
            string json = reader.ReadToEnd();

            Assert.Matches(".*\"\\$type\":\"Mvp.Selections.Domain.SelectionRole, Mvp.Selections.Domain\".*", json);
            Assert.Matches(".*\"country\":\\{\"name\":\"Country A\".*", json);
        }

        [Fact]
        public void CanDeserializeSystemRole()
        {
            // Arrange
            string json =
                "{\"$type\":\"Mvp.Selections.Domain.SystemRole, Mvp.Selections.Domain\",\"rights\":\"Apply, Review\",\"name\":\"Test\",\"id\":\"579b081b-3c39-498f-b770-9ed447a44fd4\",\"createdOn\":\"2022-09-11T00:00:00\",\"createdBy\":\"System\",\"modifiedOn\":\"2022-09-12T00:00:00\",\"modifiedBy\":\"Test\"}";

            // Act
            Role? role = JsonSerializer.Deserialize<Role>(json, GetOptions());

            // Assert
            Assert.NotNull(role);
            Assert.IsType<SystemRole>(role);
            Assert.Equal(Right.Apply | Right.Review, ((SystemRole)role!).Rights);
        }

        [Fact]
        public void CanDeserializeSelectionRole()
        {
            // Arrange
            string json =
                "{\"$type\":\"Mvp.Selections.Domain.SelectionRole, Mvp.Selections.Domain\",\"country\":{\"name\":\"Country A\",\"region\":{\"name\":\"Region A\",\"countries\":[],\"id\":1,\"createdOn\":\"2022-09-11T00:00:00+02:00\",\"createdBy\":\"System\"},\"users\":[],\"id\":1,\"createdOn\":\"2022-09-11T00:00:00+02:00\",\"createdBy\":\"System\"},\"name\":\"Test\",\"id\":\"8fd1fe8b-3724-4759-b4c8-1a3a45f3a470\",\"createdOn\":\"2022-09-11T00:00:00\",\"createdBy\":\"System\",\"modifiedOn\":\"2022-09-12T00:00:00\",\"modifiedBy\":\"Test\"}";

            // Act
            Role? role = JsonSerializer.Deserialize<Role>(json, GetOptions());

            // Assert
            Assert.NotNull(role);
            Assert.IsType<SelectionRole>(role);
            Assert.Equal("Country A", ((SelectionRole)role!).Country!.Name);
            Assert.Equal("Region A", ((SelectionRole)role).Country!.Region!.Name);
        }

        private static JsonSerializerOptions GetOptions()
        {
            JsonSerializerOptions result = new ()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            result.Converters.Add(new JsonStringEnumConverter());
            result.Converters.Add(new RoleConverter());

            return result;
        }
    }
}
