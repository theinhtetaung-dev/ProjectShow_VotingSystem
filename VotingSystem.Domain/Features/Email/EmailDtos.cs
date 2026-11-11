namespace VotingSystem.Domain.Features.Email;

public class EmailAccountCreateRequestModel
{
    public string EmailAddress { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string AppPassword { get; set; } = null!;
    public int PriorityOrder { get; set; } = 1;
    public bool IsActive { get; set; } = true;
}

public class EmailAccountUpdateRequestModel
{
    public long Id { get; set; }
    public string? EmailAddress { get; set; }
    public string? DisplayName { get; set; }
    public string? AppPassword { get; set; }
    public int? PriorityOrder { get; set; }
    public bool? IsActive { get; set; }
}

public class EmailAccountStatusUpdateRequestModel
{
    public bool IsActive { get; set; }
}

public class EmailAccountResponseModel
{
    public long Id { get; set; }
    public string EmailAddress { get; set; } = null!;
    public string? DisplayName { get; set; }
    public int PriorityOrder { get; set; }
    public bool IsActive { get; set; }
    public int TotalEmailsSent { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool HasCredential { get; set; }
}

public sealed record SelectedEmailAccount(
    long Id,
    string EmailAddress,
    string? DisplayName,
    string EncryptedAppPassword,
    int PriorityOrder);

public class EmailSendRequestModel
{
    public string ToEmail { get; set; } = null!;
    public string? ToName { get; set; }
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public bool IsHtml { get; set; } = true;
}

public class EmailSendResponseModel
{
    public long EmailAccountId { get; set; }
    public string SenderEmail { get; set; } = null!;
    public string RecipientEmail { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public DateTime SentAtUtc { get; set; }
    public int AttemptsMade { get; set; }
}
