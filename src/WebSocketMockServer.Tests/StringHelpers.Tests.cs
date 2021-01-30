using System;

using FluentAssertions;

using Xunit;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using WebSocketMockServer.Helpers;

namespace WebSocketMockServer.Tests
{
    public class StringHelpersTests
    {
        [Fact(DisplayName = "ReconvertWithJson can not process null string.")]
        [Trait("Category", "Unit")]
        public void CantConvertNullString()
        {
            //Arrange
            string request = null!;

            // Act
            var exception = Record.Exception(
                () => request.ReconvertWithJson());

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Theory(DisplayName = "ReconvertWithJson can not process empty string.")]
        [InlineData("")]
        [InlineData("  ")]
        [Trait("Category", "Unit")]
        public void CantConvertEmptyString(string request)
        {
            // Act
            var exception = Record.Exception(
                () => request.ReconvertWithJson());

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
        }

        [Fact(DisplayName = "ReconvertWithJson can not convert not json string.")]
        [Trait("Category", "Unit")]
        public void CantConvertNotJsonString()
        {
            //Arrange
            var request = "AAA";

            var exception = Record.Exception(
               () => request.ReconvertWithJson());

            // Assert
            exception.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
        }

        [Fact(DisplayName = "ReconvertWithJson skip convertation for not json string.")]
        [Trait("Category", "Unit")]
        public void ReconvertWithJsonProcessJsonString()
        {
            //Arrange
            var request = "{   \"A\": \"1\" }";

            // Act
            var request2 = request.ReconvertWithJson();

            // Assert
            var formatted = JObject.Parse(request).ToString(Formatting.Indented);
            request2.Should().BeEquivalentTo(formatted);
        }
    }
}
