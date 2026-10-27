using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess.AppDbContextModels;

[Table("Tbl_Project_Type")]
[Index("Code", Name = "UQ_Tbl_ProjectType_Code", IsUnique = true)]
[Index("Name", Name = "UQ_Tbl_ProjectType_Name", IsUnique = true)]
public partial class TblProjectType
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("code")]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("delete_flag")]
    public bool DeleteFlag { get; set; }

    [InverseProperty("ProjectType")]
    public virtual ICollection<TblProject> TblProjects { get; set; } = new List<TblProject>();

    [InverseProperty("ProjectType")]
    public virtual ICollection<TblVotingLimit> TblVotingLimits { get; set; } = new List<TblVotingLimit>();
}
