using agent_openai_banking_assistant_csharp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace banking_assistant.Tests.Controllers;

public class AuthSetupControllerTests
{
    [Fact]
    public void Index_ReturnsLoginDisabled()
    {
        // Arrange
        var controller = new AuthSetupController();

        // Act
        var result = controller.Index();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsAssignableFrom<object>(okResult.Value);
        Assert.Equivalent(new { UseLogin = false }, payload);
    }
}
