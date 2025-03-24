using agent_openai_banking_assistant_csharp.Agents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

public class AgenticRouter
{
    private IKernelBuilder _builder;
    private ChatMessageContent[] _history;
    public AgenticRouter(IKernelBuilder builder)
    {
        _builder = builder;
    }
    public async void initializeAgents()
    {
        Kernel kernel = _builder.Build();

        ChatCompletionAgent paymentAgent = new()
        {
            Instructions = AgentInstructions.PaymentAgentInstructions,
            Name = "PaymentAgent",
            Kernel = kernel,
            Arguments =
            new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
        };

        ChatCompletionAgent historyReportingAgent = new()
        {
            Instructions = AgentInstructions.TransactionsReportingAgentInstructions,
            Name = "TransactionsReportingAgent",
            Kernel = kernel,
            Arguments =
            new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
        };

        ChatCompletionAgent accountAgent = new()
        {
            Instructions = AgentInstructions.AccountAgentInstructions,
            Name = "AccountAgent",
            Kernel = kernel,
            Arguments =
            new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
        };

    
    }
}

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
class ApprovalTerminationStrategy : TerminationStrategy
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
{
    // Terminate when the final message contains the term "approve"
    protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
        => Task.FromResult(history[history.Count - 1].Content?.Contains("approve", StringComparison.OrdinalIgnoreCase) ?? false);
}