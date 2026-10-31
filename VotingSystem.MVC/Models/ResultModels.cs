using System.ComponentModel.DataAnnotations;

namespace VotingSystem.MVC.Models;

public enum ResultStatus { first_prize, second_prize, third_prize, normal }
public class ResultCreateRequestModel
{
    public long EventId { get; set; }
    public long ProjectId { get; set; }
    public ResultStatus Status { get; set; }
    [Range(1, int.MaxValue)] public int Version { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public long? ReviewedBy { get; set; }
    public DateTime? PublishedAt { get; set; }
    public long? PublishedBy { get; set; }
}
public class ResultUpdateRequestModel : ResultCreateRequestModel { public long Id { get; set; } }
public class ResultResponseModel : ResultUpdateRequestModel
{
    public DateTime CreatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public long? ModifiedBy { get; set; }
}
