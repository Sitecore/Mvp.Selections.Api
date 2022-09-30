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

        [Fact]
        public void CanDeserializeInUserArray()
        {
            // Arrange
            string json =
                "[{\"identifier\":\"00u12gh3hyyboZUgN0h8\",\"name\":\"Rob Earlam\",\"email\":\"rob.earlam@sitecore.com\",\"imageType\":\"Anonymous\",\"mentors\":[],\"applications\":[],\"consents\":[],\"links\":[],\"reviews\":[],\"roles\":[{\"$type\":\"Mvp.Selections.Domain.SystemRole, Mvp.Selections.Domain\",\"rights\":\"Apply\",\"name\":\"Candidate\",\"users\":[{\"identifier\":\"00uqyu5bxcffmH3xP0h71\",\"name\":\"Ivan Lieckens (Applicant)\",\"email\":\"ivan.lieckens@sitecore.com\",\"imageType\":\"Anonymous\",\"country\":{\"name\":\"Netherlands\",\"users\":[],\"id\":150,\"createdOn\":\"2022-09-01T00:00:00\",\"createdBy\":\"System\"},\"mentors\":[],\"applications\":[],\"consents\":[],\"links\":[],\"reviews\":[],\"roles\":[],\"titles\":[],\"rights\":\"Apply\",\"id\":\"0a90c9bf-b542-4caa-c745-08daa0818e6f\",\"createdOn\":\"2022-09-27T12:12:32.9794531\",\"createdBy\":\"00uqyu5bxcffmH3xP0h7\",\"modifiedOn\":\"2022-09-28T17:06:40.6017683\",\"modifiedBy\":\"00uqyu5bxcffmH3xP0h7\"}],\"id\":\"00000000-0000-0000-0000-000000000002\",\"createdOn\":\"2022-09-01T00:00:00\",\"createdBy\":\"System\"}],\"titles\":[],\"rights\":\"Apply\",\"id\":\"c244b683-186b-4821-3e4b-08daa0da3ee0\",\"createdOn\":\"2022-09-27T22:47:24.4719466\",\"createdBy\":\"00u12gh3hyyboZUgN0h8\"},{\"identifier\":\"00uqyu5bxcffmH3xP0h71\",\"name\":\"Ivan Lieckens (Applicant)\",\"email\":\"ivan.lieckens@sitecore.com\",\"imageType\":\"Anonymous\",\"country\":{\"name\":\"Netherlands\",\"users\":[],\"id\":150,\"createdOn\":\"2022-09-01T00:00:00\",\"createdBy\":\"System\"},\"mentors\":[],\"applications\":[],\"consents\":[],\"links\":[],\"reviews\":[],\"roles\":[{\"$type\":\"Mvp.Selections.Domain.SystemRole, Mvp.Selections.Domain\",\"rights\":\"Apply\",\"name\":\"Candidate\",\"users\":[{\"identifier\":\"00u12gh3hyyboZUgN0h8\",\"name\":\"Rob Earlam\",\"email\":\"rob.earlam@sitecore.com\",\"imageType\":\"Anonymous\",\"mentors\":[],\"applications\":[],\"consents\":[],\"links\":[],\"reviews\":[],\"roles\":[],\"titles\":[],\"rights\":\"Apply\",\"id\":\"c244b683-186b-4821-3e4b-08daa0da3ee0\",\"createdOn\":\"2022-09-27T22:47:24.4719466\",\"createdBy\":\"00u12gh3hyyboZUgN0h8\"}],\"id\":\"00000000-0000-0000-0000-000000000002\",\"createdOn\":\"2022-09-01T00:00:00\",\"createdBy\":\"System\"}],\"titles\":[],\"rights\":\"Apply\",\"id\":\"0a90c9bf-b542-4caa-c745-08daa0818e6f\",\"createdOn\":\"2022-09-27T12:12:32.9794531\",\"createdBy\":\"00uqyu5bxcffmH3xP0h7\",\"modifiedOn\":\"2022-09-28T17:06:40.6017683\",\"modifiedBy\":\"00uqyu5bxcffmH3xP0h7\"},{\"identifier\":\"00uqyu5bxcffmH3xP0h7\",\"name\":\"Ivan Lieckens (Admin)\",\"email\":\"ivan.lieckens@sitecore.com\",\"imageType\":\"Anonymous\",\"country\":{\"name\":\"Belgium\",\"users\":[],\"id\":21,\"createdOn\":\"2022-09-01T00:00:00\",\"createdBy\":\"System\"},\"mentors\":[],\"applications\":[],\"consents\":[],\"links\":[],\"reviews\":[],\"roles\":[{\"$type\":\"Mvp.Selections.Domain.SystemRole, Mvp.Selections.Domain\",\"rights\":\"Admin\",\"name\":\"Admin\",\"users\":[],\"id\":\"00000000-0000-0000-0000-000000000001\",\"createdOn\":\"2022-09-01T00:00:00\",\"createdBy\":\"System\"}],\"titles\":[],\"rights\":\"Admin\",\"id\":\"00000000-0000-0000-0000-000000000001\",\"createdOn\":\"2022-09-01T00:00:00\",\"createdBy\":\"System\"}]";

            // Act
            IList<User>? users = JsonSerializer.Deserialize<IList<User>>(json, GetOptions());

            // Assert
            Assert.NotNull(users);
            Assert.Equal(1, users?.FirstOrDefault()?.Roles?.Count);
            Assert.Equal(1, users?.FirstOrDefault()?.Roles?.FirstOrDefault()?.Users?.Count);
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
