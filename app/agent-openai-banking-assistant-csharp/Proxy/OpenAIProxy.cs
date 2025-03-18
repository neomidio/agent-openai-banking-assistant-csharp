public class OpenAIProxy : IOpenAI 
{
    private readonly OpenAIClient _client;

    public OpenAIProxy(OpenAIClient client, IConfiguration configuration)
    {
        _client = client;
    }
}

