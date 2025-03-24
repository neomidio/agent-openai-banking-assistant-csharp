using agent_openai_banking_assistant_csharp.Agents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.SemanticKernel.ChatCompletion;


[Route("api/chat")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly ILogger<ChatController> _logger;

    private RouterAgent _routerAgent;

    public ChatController(ILogger<ChatController> logger, RouterAgent routerAgent)
    {
        _logger = logger;
        this._routerAgent = routerAgent;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return StatusCode(200, "Chat Controller is available.");
    }

    [HttpPost]
    public IActionResult ChatWithOpenAI([FromBody] ChatAppRequest chatRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (chatRequest.Stream)
        {
            _logger.LogWarning(
                "Requested a content-type of application/json however also requested streaming. " +
                "Please use a content-type of application/ndjson");
            return BadRequest(
                "Requested a content-type of application/json however also requested streaming. " +
                "Please use a content-type of application/ndjson");
        }

        if (chatRequest.Messages == null || !chatRequest.Messages.Any())
        {
            _logger.LogWarning("history cannot be null in Chat request");
            return BadRequest();
        }

        ChatHistory chatHistory = ConvertSKChatHistory(chatRequest);

        _logger.LogDebug("Processing chat conversation..", chatHistory[chatHistory.Count-1].Content);

        var agentContext = new AgentContext();
        agentContext.Add("requestContext", chatRequest.Context);
        agentContext.Add("attachments", chatRequest.Attachments);
        agentContext.Add("approach", chatRequest.Approach);

        this._routerAgent.Run(chatHistory, agentContext);

        return Ok(ChatResponse.BuildChatResponse(chatHistory, agentContext));
    }

    private ChatHistory ConvertSKChatHistory(ChatAppRequest chatAppRequest)
    {
        ChatHistory chatHistory = new ChatHistory();
        foreach (var historyChat in chatAppRequest.Messages)
        {
            if ("user".Equals(historyChat.Role))
            {
                if (historyChat.Attachments == null || !historyChat.Attachments.Any())
                    chatHistory.AddUserMessage(historyChat.Content);
                else
                    chatHistory.AddUserMessage(historyChat.Content + " " + historyChat.Attachments.ToString());
            }
            if ("assistant".Equals(historyChat.Role))
                chatHistory.AddAssistantMessage(historyChat.Content);
        }
        // chatHistory.AddUserMessage(lastUserMessage.Content);

        return chatHistory;
    }
}


