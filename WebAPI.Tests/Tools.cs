using System.Text;
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

    public static IFormFile FilePathToFormFile(this string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var content = File.ReadAllBytes(filePath);
        var stream = new MemoryStream(content);
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.OpenReadStream()).Returns(stream);
        fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns((Stream target, CancellationToken token) => MockCopyTo(fileMock.Object, target, token));
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(content.Length);
        return fileMock.Object;
    }

    public static IFormFile ContentToFormFile(this string content, string fileName)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.OpenReadStream()).Returns(stream);
        fileMock.Setup(_ => _.CopyTo(It.IsAny<Stream>()))
            .Callback((Stream target) => MockCopyTo(fileMock.Object, target));
        fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns((Stream target, CancellationToken token) => MockCopyTo(fileMock.Object, target, token));
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(content.Length);
        return fileMock.Object;
    }

    private static Task MockCopyTo(IFormFile formFile, Stream target, CancellationToken? token = null)
    {
        var fs = formFile.OpenReadStream();
        fs.Position = 0;
        fs.CopyTo(target);
        return Task.CompletedTask;
    }
}
