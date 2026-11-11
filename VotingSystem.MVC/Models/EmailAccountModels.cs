using System.ComponentModel.DataAnnotations;

namespace VotingSystem.MVC.Models;

public abstract class EmailAccountFormModel
{
    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(255)]
    [Display(Name = "Email address")]
    public string EmailAddress { get; set; } = string.Empty;

    [StringLength(150)]
    [Display(Name = "Sender display name")]
    public string? DisplayName { get; set; }

    [Range(1, 100000, ErrorMessage = "Priority order must be at least 1.")]
    [Display(Name = "Priority order")]
    public int PriorityOrder { get; set; } = 1;

    [Display(Name = "Active in Round Robin")]
    public bool IsActive { get; set; } = true;
}

public class EmailAccountCreateRequestModel : EmailAccountFormModel
{
    [Required(ErrorMessage = "Gmail App Password is required.")]
    [StringLength(256)]
    [DataType(DataType.Password)]
    [Display(Name = "App Password")]
    public string AppPassword { get; set; } = string.Empty;
}

public class EmailAccountUpdateRequestModel : EmailAccountFormModel
{
    public long Id { get; set; }

    [StringLength(256)]
    [DataType(DataType.Password)]
    [Display(Name = "New App Password (optional)")]
    public string? AppPassword { get; set; }
}

public class EmailAccountStatusUpdateRequestModel
{
    public bool IsActive { get; set; }
}

public class EmailAccountResponseModel
{
    public long Id { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public int PriorityOrder { get; set; }
    public bool IsActive { get; set; }
    public int TotalEmailsSent { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool HasCredential { get; set; }
}
