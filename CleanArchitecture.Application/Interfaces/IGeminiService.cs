namespace CleanArchitecture.Application.Interfaces
{
    public interface IGeminiService
    {
        public Task<string> GenerateContentAsync(Guid UserId, string text);
    }
}
