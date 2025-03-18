namespace agent_openai_banking_assistant_csharp.Interfaces
{
    public interface IBlobStorage
    {
        public Task<string> GetFileAsString(String fileName);
        public Task StoreFile(String fileName, Stream content);
    }
}
