using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Domain.Features.Event;

public  class EventDtos
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime EventStartAt { get; set; }
    public DateTime EventEndAt { get; set; }
    public DateTime VotingStartAt { get; set; }
    public DateTime VotingEndAt { get; set; }
    public DateTime? ResultShowStartAt { get; set; }
    public DateTime? ResultShowEndAt { get; set; }
    public string Status { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool DeleteFlag { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public long? ModifiedBy { get; set; }
}

public class EventCreateRequestModel
{
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime VotingStartAt { get; set; }
    public DateTime VotingEndAt { get; set; }
    public DateTime? ResultShowStartAt { get; set; }
    public DateTime? ResultShowEndAt { get; set; }
    public string Status { get; set; } = null!;
}

public class EventUpdateRequestModel
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? VotingStartAt { get; set; }
    public DateTime? VotingEndAt { get; set; }
    public DateTime? ResultShowStartAt { get; set; }
    public DateTime? ResultShowEndAt { get; set; }
    public string? Status { get; set; }
}

public class EventResponseModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime EventStartAt { get; set; }
    public DateTime EventEndAt { get; set; }

    public DateTime VotingStartAt { get; set; }
    public DateTime VotingEndAt { get; set; }

    public DateTime? ResultShowStartAt { get; set; }
    public DateTime? ResultShowEndAt { get; set; }

    public string Status { get; set; } = null!;
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public long? ModifiedBy { get; set; }
}
