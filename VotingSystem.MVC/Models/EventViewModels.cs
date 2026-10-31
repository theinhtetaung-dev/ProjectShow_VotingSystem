using System.ComponentModel.DataAnnotations;

namespace VotingSystem.MVC.Models;

public abstract class EventRequestModel : IValidatableObject
{
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Code { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required, Display(Name = "Event start")]
    public DateTime StartDate { get; set; }

    [Required, Display(Name = "Event end")]
    public DateTime EndDate { get; set; }

    [Required, Display(Name = "Voting start")]
    public DateTime VotingStartAt { get; set; }

    [Required, Display(Name = "Voting end")]
    public DateTime VotingEndAt { get; set; }

    [Display(Name = "Result show start")]
    public DateTime? ResultShowStartAt { get; set; }

    [Display(Name = "Result show end")]
    public DateTime? ResultShowEndAt { get; set; }

    [Required]
    public string Status { get; set; } = "Upcoming";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate <= StartDate)
            yield return new ValidationResult("Event end must be later than event start.", [nameof(EndDate)]);

        if (VotingEndAt <= VotingStartAt)
            yield return new ValidationResult("Voting end must be later than voting start.", [nameof(VotingEndAt)]);

        if (VotingStartAt < StartDate || VotingEndAt > EndDate)
            yield return new ValidationResult("Voting dates must be within the event date range.", [nameof(VotingStartAt), nameof(VotingEndAt)]);

        if (ResultShowStartAt.HasValue != ResultShowEndAt.HasValue)
            yield return new ValidationResult("Both result show dates are required when either one is entered.", [nameof(ResultShowStartAt), nameof(ResultShowEndAt)]);

        if (ResultShowStartAt.HasValue && ResultShowEndAt <= ResultShowStartAt)
            yield return new ValidationResult("Result show end must be later than result show start.", [nameof(ResultShowEndAt)]);
    }
}

public class EventCreateRequestModel : EventRequestModel
{
}

public class EventUpdateRequestModel : EventRequestModel
{
    public long Id { get; set; }
}

public class EventResponseModel
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EventStartAt { get; set; }
    public DateTime EventEndAt { get; set; }
    public DateTime VotingStartAt { get; set; }
    public DateTime VotingEndAt { get; set; }
    public DateTime? ResultShowStartAt { get; set; }
    public DateTime? ResultShowEndAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class ApiResult<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}
