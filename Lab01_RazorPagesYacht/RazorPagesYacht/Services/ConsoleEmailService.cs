namespace RazorPagesYacht.Services;

// "Fake" sender for development: writes the email to the server log instead of sending it
public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;

    public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string toEmail, string subject, string htmlBody, string? plainText = null)
    {
        _logger.LogInformation("[EMAIL] To: {To} | Subject: {Subject}\n{Body}",
            toEmail, subject, plainText ?? htmlBody);
        return Task.CompletedTask;
    }
}