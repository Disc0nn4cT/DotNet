namespace RazorPagesYacht.Services;

public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string htmlBody, string? plainText = null);
}