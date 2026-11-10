using System;
using System.Collections.Generic;
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

public class EventDashboardResponseModel
{
    public EventResponseModel Event { get; set; } = null!;
    public List<EventPersonResponseModel> People { get; set; } = [];
    public List<EventUserOptionResponseModel> AvailableUsers { get; set; } = [];
    public List<EventUserOptionResponseModel> AvailableAdmins { get; set; } = [];
    public List<EventVotingAllocationResponseModel> VotingAllocations { get; set; } = [];
    public List<EventProjectResponseModel> Projects { get; set; } = [];
    public List<EventProjectTypeResponseModel> ProjectTypes { get; set; } = [];
}

public class EventPersonResponseModel
{
    public long UserId { get; set; }
    public long? UserEventId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string AssignmentType { get; set; } = null!;
    public string? AccessStatus { get; set; }
    public DateTime AssignedAt { get; set; }
}

public class EventUserOptionResponseModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
}

public class EventVotingAllocationResponseModel
{
    public long UserEventId { get; set; }
    public long UserId { get; set; }
    public string UserName { get; set; } = null!;
    public int ProjectTypeId { get; set; }
    public string ProjectTypeName { get; set; } = null!;
    public int VoteLimit { get; set; }
    public int VoteUsed { get; set; }
    public int VoteRemaining { get; set; }
}

public class EventProjectResponseModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string ProjectTypeName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int VoteCounts { get; set; }
}

public class EventProjectTypeResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
