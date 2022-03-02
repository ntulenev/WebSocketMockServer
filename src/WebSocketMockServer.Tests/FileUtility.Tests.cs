using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

using Moq;

using WebSocketMockServer.IO;

using Xunit;

namespace WebSocketMockServer.Tests
{
    public class FileUtilityTests
    {
        [Fact(DisplayName = "FileUtility can not process null Environment.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileUtilityWithNullEnvironment()
        {
            // Act
            var exception = Record.Exception(
                () => new FileUtility(null!, Mock.Of<ILogger<FileUtility>>()));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "FileUtility can not process null Logger.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileUtilityWithNullLogger()
        {
            // Act
            var exception = Record.Exception(
                () => new FileUtility(Mock.Of<IWebHostEnvironment>(), null!));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "FileUtility can be certed.")]
        [Trait("Category", "Unit")]
        public void CanCreateFileUtility()
        {
            // Act
            var exception = Record.Exception(
                () => new FileUtility(Mock.Of<IWebHostEnvironment>(), Mock.Of<ILogger<FileUtility>>()));

            // Assert
            exception.Should().BeNull();
        }
    }
}
