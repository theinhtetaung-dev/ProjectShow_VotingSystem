namespace VotingSystem.Domain.Features.Email;

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string DefaultFromName { get; set; } = "NEXUS Vote";
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxRetryAttempts { get; set; } = 5;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Host))
        {
            throw new InvalidOperationException("Smtp:Host is required.");
        }

        if (Port is <= 0 or > 65535)
        {
            throw new InvalidOperationException("Smtp:Port must be between 1 and 65535.");
        }

        if (TimeoutSeconds <= 0)
        {
            throw new InvalidOperationException("Smtp:TimeoutSeconds must be greater than zero.");
        }

        if (MaxRetryAttempts <= 0)
        {
            throw new InvalidOperationException("Smtp:MaxRetryAttempts must be greater than zero.");
        }
    }
}
