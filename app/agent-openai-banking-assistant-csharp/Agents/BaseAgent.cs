using Microsoft.SemanticKernel.ChatCompletion;

public interface BaseAgent
{
    public void Run(ChatHistory userChatHistory, AgentContext agentContext);
}

