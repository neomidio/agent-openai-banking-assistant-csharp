using System.Collections.Generic;
using agent_openai_banking_assistant_csharp.Controllers;
using agent_openai_banking_assistant_csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace banking_assistant.Tests.Controllers;

public class ChatControllerTests
{
    private static ChatController CreateController(Mock<IAgentRouter>? routerMock = null)
    {
        var loggerMock = new Mock<ILogger<ChatController>>();
        routerMock ??= new Mock<IAgentRouter>();
        return new ChatController(loggerMock.Object, routerMock.Object);
    }

    [Fact]
    public void Index_ReturnsAvailabilityMessage()
    {
        var controller = CreateController();

        var result = controller.Index();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Chat Controller is available.", okResult.Value);
    }

    [Fact]
    public void ChatWithOpenAI_InvalidModel_ReturnsBadRequest()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError("test", "invalid");
        var request = BuildValidRequest();

        var result = controller.ChatWithOpenAI(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void ChatWithOpenAI_StreamRequest_ReturnsBadRequest()
    {
        var controller = CreateController();
        var request = BuildValidRequest() with { Stream = true };

        var result = controller.ChatWithOpenAI(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("application/ndjson", badRequest.Value?.ToString());
    }

    [Fact]
    public void ChatWithOpenAI_MissingMessages_ReturnsBadRequest()
    {
        var controller = CreateController();
        var request = BuildValidRequest() with { Messages = new List<ResponseMessage>() };

        var result = controller.ChatWithOpenAI(request);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void ChatWithOpenAI_ValidRequest_ReturnsChatResponse()
    {
        var routerMock = new Mock<IAgentRouter>();
        routerMock
            .Setup(r => r.Run(It.IsAny<ChatHistory>(), It.IsAny<AgentContext>()))
            .Returns(Task.CompletedTask)
            .Callback<ChatHistory, AgentContext>((history, context) =>
            {
                history.AddAssistantMessage("Respuesta generada");
                context.Add("dataPoints", new List<string> { "Transacción 123" });
                context.Add("thoughts", "Pensando... ");
                context.Add("attachments", new List<string> { "comprobante.pdf" });
            });

        var controller = CreateController(routerMock);
        var request = BuildValidRequest();

        var result = controller.ChatWithOpenAI(request);

        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<ChatResponse>(jsonResult.Value);
        var choice = Assert.Single(response.Choices);
        Assert.Equal("assistant", choice.Message.Role);
        Assert.Equal("Respuesta generada", choice.Message.Content);
        Assert.Contains("Transacción 123", choice.Context.DataPoints);
        Assert.Equal("Pensando... ", choice.Context.Thoughts);
        Assert.Contains("comprobante.pdf", choice.Message.Attachments!);
        routerMock.Verify(r => r.Run(It.IsAny<ChatHistory>(), It.IsAny<AgentContext>()), Times.Once);
    }

    private static ChatAppRequest BuildValidRequest()
    {
        return new ChatAppRequest(
            Messages: new List<ResponseMessage>
            {
                new("Necesito ayuda con mi cuenta", "user", new List<string> { "comprobante.pdf" })
            },
            Stream: false,
            Context: new ChatAppRequestContext(new ChatAppRequestOverrides(null, null, null, null, null, null, null, null, null, null, null, null)),
            Attachments: new List<string>(),
            Approach: "default");
    }
}
