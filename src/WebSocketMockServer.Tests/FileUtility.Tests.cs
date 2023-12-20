using FluentAssertions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

using Moq;

using WebSocketMockServer.IO;

using Xunit;

namespace WebSocketMockServer.Tests;

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
            () => new FileUtility(Mock.Of<IWebHostEnvironment>(MockBehavior.Strict), null!));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "FileUtility can be certed.")]
    [Trait("Category", "Unit")]
    public void CanCreateFileUtility()
    {
        // Act
        var exception = Record.Exception(
            () => new FileUtility(Mock.Of<IWebHostEnvironment>(MockBehavior.Strict), Mock.Of<ILogger<FileUtility>>()));

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "FileUtility can not process null path.")]
    [Trait("Category", "Unit")]
    public async Task CantReadFileWithNullPathAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var fileUtility = new FileUtility(
            Mock.Of<IWebHostEnvironment>(MockBehavior.Strict),
            Mock.Of<ILogger<FileUtility>>());
        // Act
        var exception = await Record.ExceptionAsync(async
                        () => await fileUtility.ReadFileAsync(null!, "name", cts.Token));
        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "FileUtility can not process null name.")]
    [Trait("Category", "Unit")]
    public async Task CantReadFileWithNullNameAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var fileUtility = new FileUtility(
                       Mock.Of<IWebHostEnvironment>(MockBehavior.Strict),
                                  Mock.Of<ILogger<FileUtility>>());
        // Act
        var exception = await Record.ExceptionAsync(async
                                   () => await fileUtility.ReadFileAsync("path", null!, cts.Token));
        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "FileUtility can not process with Cancelled token.")]
    [Trait("Category", "Unit")]
    public async Task CantReadFileWithCancelledTokenAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var fileUtility = new FileUtility(
                                  Mock.Of<IWebHostEnvironment>(MockBehavior.Strict),
                                  Mock.Of<ILogger<FileUtility>>());
        // Act
        var exception = await Record.ExceptionAsync(async
                                              () => await fileUtility.ReadFileAsync("path", "name", cts.Token));
        // Assert
        exception.Should().NotBeNull().And.BeOfType<OperationCanceledException>();
    }
}
