using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Result;

public class ResultDtos
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public long ProjectId { get; set; }
    public Enums.ResultStatus Status { get; set; }
    public int Version { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public long? ReviewedBy { get; set; }
    public DateTime? PublishedAt { get; set; }
    public long? PublishedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public long? ModifiedBy { get; set; }
    public bool DeleteFlag { get; set; }
}

public class ResultResponseModel
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public long ProjectId { get; set; }
    public Enums.ResultStatus Status { get; set; }
    public int Version { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public long? ReviewedBy { get; set; }
    public DateTime? PublishedAt { get; set; }
    public long? PublishedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public long? ModifiedBy { get; set; }
}

public class ResultCreateRequestModel
{
    public long EventId { get; set; }
    public long ProjectId { get; set; }
    public Enums.ResultStatus Status { get; set; }
    public int Version { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public long? ReviewedBy { get; set; }
    public DateTime? PublishedAt { get; set; }
    public long? PublishedBy { get; set; }
}

public class ResultUpdateRequestModel
{
    public long Id { get; set; }
    public long? EventId { get; set; }
    public long? ProjectId { get; set; }
    public Enums.ResultStatus? Status { get; set; }
    public int? Version { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public long? ReviewedBy { get; set; }
    public DateTime? PublishedAt { get; set; }
    public long? PublishedBy { get; set; }
}
