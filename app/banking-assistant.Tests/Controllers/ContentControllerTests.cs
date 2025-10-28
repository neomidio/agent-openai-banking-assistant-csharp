using System.IO;
using System.Text;
using agent_openai_banking_assistant_csharp.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace banking_assistant.Tests.Controllers;

public class ContentControllerTests
{
    private static ContentController CreateController(Mock<IBlobStorage>? storageMock = null)
    {
        var environmentMock = new Mock<IWebHostEnvironment>();
        var loggerMock = new Mock<ILogger<ContentController>>();
        storageMock ??= new Mock<IBlobStorage>();
        return new ContentController(environmentMock.Object, storageMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task Index_WithValidFile_ReturnsFileContent()
    {
        var storageMock = new Mock<IBlobStorage>();
        storageMock.Setup(s => s.GetFileAsBytesAsync("comprobante.png"))
            .ReturnsAsync(Encoding.UTF8.GetBytes("archivo"));
        var controller = CreateController(storageMock);

        var result = await controller.Index("comprobante.png");

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("image/png", fileResult.ContentType);
        Assert.Equal("archivo", Encoding.UTF8.GetString(fileResult.FileContents));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Index_WithMissingFileName_ReturnsBadRequest(string? fileName)
    {
        var controller = CreateController();

        var result = await controller.Index(fileName!);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No file name provided.", badRequest.Value);
    }

    [Fact]
    public async Task Index_WhenStorageThrows_ReturnsServerError()
    {
        var storageMock = new Mock<IBlobStorage>();
        storageMock.Setup(s => s.GetFileAsBytesAsync("comprobante.png"))
            .ThrowsAsync(new InvalidOperationException("error"));
        var controller = CreateController(storageMock);

        var result = await controller.Index("comprobante.png");

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
    }

    [Fact]
    public async Task UploadContent_WithMissingFile_ReturnsBadRequest()
    {
        var controller = CreateController();

        var result = await controller.UploadContent(null!);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("File is missing.", badRequest.Value);
    }

    [Fact]
    public async Task UploadContent_WithValidFile_StoresAndReturnsName()
    {
        var storageMock = new Mock<IBlobStorage>();
        var controller = CreateController(storageMock);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("contenido"));
        var formFile = new FormFile(stream, 0, stream.Length, "file", "comprobante.pdf");

        var result = await controller.UploadContent(formFile);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("comprobante.pdf", okResult.Value);
        storageMock.Verify(s => s.StoreFileAsync("comprobante.pdf", It.IsAny<Stream>()), Times.Once);
    }

    [Fact]
    public async Task UploadContent_WhenStorageThrows_ReturnsServerError()
    {
        var storageMock = new Mock<IBlobStorage>();
        storageMock.Setup(s => s.StoreFileAsync(It.IsAny<string>(), It.IsAny<Stream>()))
            .ThrowsAsync(new InvalidOperationException("error"));
        var controller = CreateController(storageMock);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("contenido"));
        var formFile = new FormFile(stream, 0, stream.Length, "file", "comprobante.pdf");

        var result = await controller.UploadContent(formFile);

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
    }
}
