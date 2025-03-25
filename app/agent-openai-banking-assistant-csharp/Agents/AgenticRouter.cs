using agent_openai_banking_assistant_csharp.Agents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using System.Text.Json;

public class AgenticRouter
{
    private IntentExtractorAgent _intentExtractorAgent;
    private PaymentAgent _paymentAgent;
    private Kernel _kernel;
    public AgenticRouter(Kernel kernel, IConfiguration configuration, IDocumentScanner documentScanner)
    {
        this._kernel = kernel;
        this._intentExtractorAgent = new IntentExtractorAgent(kernel, configuration);
        this._paymentAgent = new PaymentAgent(kernel, configuration, documentScanner);
    }
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public async Task Run(ChatHistory chatHistory, AgentContext agentContext)
    {
        Console.WriteLine("======== Router Agent: Starting ========");
        Console.WriteLine(chatHistory[chatHistory.Count - 1].Content);
        IntentResponse intentResponse = this._intentExtractorAgent.Run(chatHistory).Result;
        Console.WriteLine($"Intent Type for chat conversation is {intentResponse.intentType.ToString()}");

        ChatCompletionAgent clarifyingAgent =
        new()
        {
            Name = "ClarifyingAgent",
            Instructions = "You are a personal financial advisor who help bank customers manage their banking accounts and services.\r\nThe user may need help with his recurrent bill payments, it may start the payment checking payments history for a specific payee.\r\nIn other cases it may want to just review account details or transactions history.\r\nBased on the conversation you need to identify the user intent.\r\nThe available intents are:\r\n\"BillPayment\",\"RepeatTransaction\",\"TransactionHistory\",\"AccountInfo\"\r\nIf none of the intents are identified provide the user with the list of the available intents. If you don't understand or if an intent is not identified be polite with the user, ask clarifying question also using the list of the available intents..\r\n",
            Kernel = this._kernel
        };


        KernelFunction selectionFunction =
        AgentGroupChat.CreatePromptFunctionForStrategy(
            $$$"""
            Determine which participant takes the next turn in a conversation based on the the intent classified.
            State only the name of the participant to take the next turn.

            Choose only from these participants:
            - 'ClarifyingAgent' for: None
            - 'PaymentAgent' for: BillPayment, RepeatTransaction
            - 'AccountAgent' for: AccountInfo
            - 'TransactionsReportingAgent' for: TransactionHistory

            Intent Classified:
            {{{intentResponse.intentType.ToString()}}}

            History:
            {{$history}}
            """,
            safeParameterNames: "history");

        KernelFunction terminationFunction =
            AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                    Examine the RESPONSE and determine whether the content has been deemed satisfactory.
                    Only if it contains 'success'.
                    If no correction is suggested, it is satisfactory.

                    RESPONSE:
                    {{$history}}
                    """,
                safeParameterNames: "history");

        // Define the selection strategy
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        KernelFunctionSelectionStrategy selectionStrategy =
          new(selectionFunction, this._kernel)
          {   
              // Parse the function response.
              ResultParser = (result) => result.GetValue<string>() ?? "",
              HistoryVariableName = "history",
              // Save tokens by not including the entire history in the prompt
              HistoryReducer = new ChatHistoryTruncationReducer(41),
          };
        // Create a chat using the defined selection strategy.
        KernelFunctionTerminationStrategy terminationStrategy =
            new(terminationFunction, this._kernel)
            {
                // Save tokens by only including the final response
                HistoryReducer = new ChatHistoryTruncationReducer(1),
                // The prompt variable name for the history argument.
                HistoryVariableName = "history",
                // Limit total number of turns
                MaximumIterations = 3,
                // Customer result parser to determine if the response is "yes"
                ResultParser = (result) => result.GetValue<string>()?.Contains("yes", StringComparison.OrdinalIgnoreCase) ?? false
            };
        #pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


        AgentGroupChat chat =
            new(this._paymentAgent.agent, clarifyingAgent)
            {
                ExecutionSettings = new() { SelectionStrategy = selectionStrategy, TerminationStrategy = terminationStrategy }
            };

        chat.IsComplete = false;

        try
        {
            await foreach (ChatMessageContent response in chat.InvokeAsync())
            {

                Console.WriteLine();
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                Console.WriteLine($"{response.AuthorName.ToUpperInvariant()}:{Environment.NewLine}{response.Content}");
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                chatHistory.AddAssistantMessage(response.Content);
             
            }
        }
        catch (HttpOperationException exception)
        {
            Console.WriteLine(exception.Message);
            if (exception.InnerException != null)
            {
                Console.WriteLine(exception.InnerException.Message);
                if (exception.InnerException.Data.Count > 0)
                {
                    Console.WriteLine(JsonSerializer.Serialize(exception.InnerException.Data, new JsonSerializerOptions() { WriteIndented = true }));
                }
            }
        }
    }
}