using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Mailexam.Services;

public class MailService
{
    public async Task SendTestAsync(string? to, string? subject, string? body, CancellationToken ct = default)
    {
        var login = Environment.GetEnvironmentVariable("MAILEXAM_LOGIN")
            ?? throw new InvalidOperationException("MAILEXAM_LOGIN is not set");
        var password = Environment.GetEnvironmentVariable("MAILEXAM_PASSWORD")
            ?? throw new InvalidOperationException("MAILEXAM_PASSWORD is not set");

        var port = int.Parse(Environment.GetEnvironmentVariable("MAILEXAM_PORT") ?? "587");
        var from = Environment.GetEnvironmentVariable("MAIL_FROM") ?? "noreply@example.test";

        to ??= "user@example.test";
        subject ??= "ASP.NET Core + Mailexam";
        body ??= "Mailexam test from ASP.NET Core";

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(from));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync($"{login}.mailexam.io", port, GetSecureSocketOptions(port), ct);
        await client.AuthenticateAsync(login, password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }

    private static SecureSocketOptions GetSecureSocketOptions(int port) => port switch
    {
        465 => SecureSocketOptions.SslOnConnect,
        25 => SecureSocketOptions.None,
        _ => SecureSocketOptions.StartTls,
    };
}
