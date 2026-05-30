using Mailexam.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<MailService>();

var app = builder.Build();

app.MapPost("/mail/test", async (SendRequest request, MailService mail, CancellationToken ct) =>
{
    await mail.SendTestAsync(request.To, request.Subject, request.Body ?? request.Text, ct);
    return Results.Ok(new { status = "ok" });
});

var host = Environment.GetEnvironmentVariable("HTTP_HOST") ?? "127.0.0.1";
var port = Environment.GetEnvironmentVariable("HTTP_PORT") ?? "8080";
app.Run($"http://{host}:{port}");

public record SendRequest(string? To, string? Subject, string? Body, string? Text);
