using Microsoft.AspNetCore.Http;
using Moq;

namespace WebAPI.Tests;

internal static class Tools
{
    public static string GetProjectDirPath(this string dirName)
    {
        return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", dirName));
    }

    public static string GetSamplePath(this string fileName)
    {
        return Path.Combine("Samples".GetProjectDirPath(), fileName);
    }

    public static IFormFile ToFormFile(this string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var content = File.ReadAllBytes(filePath);
        var stream = new MemoryStream(content)
        {
            Position = 0
        };
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.OpenReadStream()).Returns(stream);
        fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns((Stream target, CancellationToken token) => MockCopyToAsync(fileMock.Object, target, token));
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(content.Length);
        return fileMock.Object;
    }

    private static Task MockCopyToAsync(IFormFile formFile, Stream target, CancellationToken? token = null)
    {
        using var fs = formFile.OpenReadStream();
        fs.CopyTo(target);
        return Task.CompletedTask;
    }
}
