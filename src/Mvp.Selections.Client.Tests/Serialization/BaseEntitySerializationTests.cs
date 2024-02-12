using System.Text.Json;
using Mvp.Selections.Domain.Comments;

namespace Mvp.Selections.Client.Tests.Serialization
{
    public class BaseEntitySerializationTests
    {
        [Fact]
        public void CanDeserializeCreatedOn()
        {
            // Arrange
            string json =
                "{\r\n        \"value\": \"Test comment datetime\",\r\n        \"user\": {\r\n            \"identifier\": \"00uqyu5bxcffmH3xP0h7\",\r\n            \"name\": \"Ivan Lieckens\",\r\n            \"imageType\": \"Gravatar\",\r\n            \"imageUri\": \"https://www.gravatar.com/avatar/b3b05bd1d36c1b122ea08e0fc098d25e\",\r\n            \"country\": {\r\n                \"name\": \"Belgium\",\r\n                \"users\": [],\r\n                \"id\": 21,\r\n                \"createdOn\": \"2022-09-01T00:00:00\",\r\n                \"createdBy\": \"System\",\r\n                \"modifiedOn\": \"2022-11-30T13:38:05.0066667\",\r\n                \"modifiedBy\": \"sebw\"\r\n            },\r\n            \"id\": \"aa3a18eb-a7e0-4ccf-7a8b-08db37409299\",\r\n            \"createdOn\": \"2023-04-07T08:23:51.5276155\",\r\n            \"createdBy\": \"00uqyu5bxcffmH3xP0h7\",\r\n            \"modifiedOn\": \"2023-04-07T15:41:21.1232705\",\r\n            \"modifiedBy\": \"00uqyu5bxcffmH3xP0h7\"\r\n        },\r\n        \"id\": \"95b92d0d-e3fa-4540-3b17-08dc20e77821\",\r\n        \"createdOn\": \"2024-01-29T16:29:30.7059247\",\r\n        \"createdBy\": \"00uqyu5bxcffmH3xP0h7\"\r\n    }";
            
            // Act
            ApplicationComment? comment = JsonSerializer.Deserialize<ApplicationComment>(json, SerializationSettings.GetOptions());
            
            // Assert
            Assert.NotNull(comment);
            Assert.Equal(DateTime.Parse("2024-01-29T16:29:30.7059247") , comment?.CreatedOn);
        }
    }
}
