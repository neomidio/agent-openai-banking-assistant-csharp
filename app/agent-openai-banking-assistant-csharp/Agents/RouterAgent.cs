using Microsoft.SemanticKernel.ChatCompletion;

namespace agent_openai_banking_assistant_csharp.Agents
{
    public class RouterAgent
    {
        private IntentExtractorAgent _intentExtractorAgent;

        public RouterAgent(IntentExtractorAgent intentExtractorAgent)
        {
           this._intentExtractorAgent = intentExtractorAgent;
        }
        public void Run(ChatHistory chatHistory, AgentContext agentContext)
        {
            Console.WriteLine("======== Router Agent: Starting ========");
            IntentResponse intentResponse = this._intentExtractorAgent.Run(chatHistory).Result;
            Console.WriteLine($"Intent Type for chat conversation is {intentResponse.intentType.ToString()}");
        }
    }

}
