
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using System.IO;
public class PaymentAgent
{
    public ChatCompletionAgent agent;
    public PaymentAgent(Kernel kernel, IConfiguration configuration, IDocumentScanner documentScanner)
    {
        Kernel toolKernel = kernel.Clone();

        var transactionApiURL = configuration["BackendAPIs:TransactionsApiUrl"];
        var accountsApiURL = configuration["BackendAPIs:AccountsApiUrl"];
        var paymentsApiURL = configuration["BackendAPIs:PaymentsApiUrl"];

        AgenticUtils.AddOpenAPIPlugin(
           kernel: toolKernel,
           pluginName: "TransactionHistoryPlugin",
           apiName: "transaction-history",
           apiUrl: transactionApiURL
        );

        AgenticUtils.AddOpenAPIPlugin(
           kernel: toolKernel,
           pluginName: "AccountsPlugin",
           apiName: "account",
           apiUrl: accountsApiURL
        );

        AgenticUtils.AddOpenAPIPlugin(
           kernel: toolKernel,
           pluginName: "PaymentsPlugin",
           apiName: "payments",
           apiUrl: paymentsApiURL
        );

        toolKernel.Plugins.AddFromObject(new InvoiceScanPlugin(documentScanner), "InvoiceScanPlugin");
       
        this.agent =
        new()
        {
            Name = "PaymentAgent",
            Instructions = AgentInstructions.PaymentAgentInstructions,
            Kernel = toolKernel,
            Arguments =
            new KernelArguments(
                new AzureOpenAIPromptExecutionSettings(){ FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }
            )
        };
    }
}
