using Azure.Core;
using Azure.Identity;

namespace agent_openai_banking_assistant_csharp.Configurations
{
    public class CredentialConfiguration
    {
       public static TokenCredential getCredential(WebApplicationBuilder builder)
        {
            TokenCredential credential;

            if (builder.Environment.IsProduction() || builder.Environment.IsStaging())
            {
                string? clientId = builder.Configuration["UserAssignedClientId"];
                credential = new ManagedIdentityCredential(
                    ManagedIdentityId.FromUserAssignedClientId(clientId));
            }
            else
            {
                // local development environment
                credential = new ChainedTokenCredential(
                    new VisualStudioCredential(),
                    new AzureCliCredential(),
                    new AzurePowerShellCredential());
            }
            return credential;
        }
    }
}
