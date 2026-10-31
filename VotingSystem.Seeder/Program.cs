using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess.AppDbContextModels;

const string password = "Thanks!12@";
const string defaultConnection = "Server=.;Database=VotingSystemDb;User Id=sa;Password=sasa@123;TrustServerCertificate=True;";
var connectionString = Environment.GetEnvironmentVariable("VOTING_SYSTEM_DB") ?? defaultConnection;
var outputPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "VotingSystem_User_Credentials.txt"));

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer(connectionString)
    .Options;
await using var db = new AppDbContext(options);
await db.Database.OpenConnectionAsync();

var roles = await db.TblRoles.ToDictionaryAsync(x => x.Name, StringComparer.OrdinalIgnoreCase);
foreach (var requiredRole in new[] { "SuperAdmin", "Admin", "User" })
{
    if (!roles.ContainsKey(requiredRole))
        throw new InvalidOperationException($"Required role '{requiredRole}' does not exist. Run Db_V2.sql first.");
}

var nameStarts = new[]
{
    "Aung", "Kyaw", "Min", "Htet", "Hein", "Ye", "Nay", "Sai", "Myo", "Zaw",
    "Thant", "Pyae", "Phyo", "Lin", "Thiha"
};
var nameEnds = new[]
{
    "Min Khant", "Htet Aung", "Ko Ko", "Zin Oo", "Naing Win",
    "Thura Aung", "Moe Kyaw", "Wai Yan", "Kaung Myat", "Thu Ya",
    "Nandar Hlaing", "Su Mon", "Thiri Win", "May Zin", "Ei Ei"
};

var accounts = new List<AccountSeed>();
var number = 0;
foreach (var start in nameStarts)
foreach (var end in nameEnds)
{
    number++;
    var role = number <= 5 ? "SuperAdmin" : number <= 25 ? "Admin" : "User";
    var name = $"{start} {end}";
    var email = $"{Normalize(name)}@gmail.com";
    accounts.Add(new AccountSeed(name, email, role));
}

var hasher = new PasswordHasher<TblUser>();
var existingUsers = await db.TblUsers.ToDictionaryAsync(x => x.Email, StringComparer.OrdinalIgnoreCase);
foreach (var account in accounts)
{
    if (!existingUsers.TryGetValue(account.Email, out var user))
    {
        user = new TblUser
        {
            Email = account.Email,
            Name = account.Name,
            RoleId = roles[account.Role].Id,
            IsActive = true,
            DeleteFlag = false,
            CreatedAt = DateTime.UtcNow
        };
        db.TblUsers.Add(user);
        existingUsers[account.Email] = user;
    }
    else
    {
        user.Name = account.Name;
        user.RoleId = roles[account.Role].Id;
        user.IsActive = true;
        user.DeleteFlag = false;
        user.ModifiedAt = DateTime.UtcNow;
    }
    user.PasswordHash = hasher.HashPassword(user, password);
}
await db.SaveChangesAsync();

var seededUsers = await db.TblUsers
    .Where(x => accounts.Select(a => a.Email).Contains(x.Email))
    .ToDictionaryAsync(x => x.Email, StringComparer.OrdinalIgnoreCase);
var superAdmin = seededUsers[accounts[0].Email];
var admins = accounts.Where(x => x.Role == "Admin").Select(x => seededUsers[x.Email]).ToList();
var voters = accounts.Where(x => x.Role == "User").Select(x => seededUsers[x.Email]).ToList();
var projectTypes = await db.TblProjectTypes.Where(x => x.IsActive && !x.DeleteFlag).OrderBy(x => x.Id).Take(3).ToListAsync();
if (projectTypes.Count < 3) throw new InvalidOperationException("At least three active project types are required.");

var firstMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-12);
for (var month = 0; month < 12; month++)
{
    var monthStart = firstMonth.AddMonths(month);
    var code = $"YEAR-SEED-{monthStart:yyyyMM}";
    var votingStart = monthStart.AddDays(14).AddHours(9);
    var votingEnd = monthStart.AddDays(18).AddHours(17);
    var eventEntity = await db.TblEvents.FirstOrDefaultAsync(x => x.Code == code);
    if (eventEntity == null)
    {
        eventEntity = new TblEvent
        {
            Name = $"Myanmar Community Innovation Vote - {monthStart:MMMM yyyy}",
            Code = code,
            Description = "Monthly community project voting event generated for the one-year reporting dataset.",
            EventStartAt = monthStart.AddDays(7),
            EventEndAt = monthStart.AddDays(21).AddHours(18),
            VotingStartAt = votingStart,
            VotingEndAt = votingEnd,
            ResultShowStartAt = votingEnd.AddDays(1),
            ResultShowEndAt = votingEnd.AddMonths(6),
            Status = "Published",
            IsActive = true,
            DeleteFlag = false,
            CreatedAt = monthStart.AddDays(1),
            CreatedBy = superAdmin.Id
        };
        db.TblEvents.Add(eventEntity);
        await db.SaveChangesAsync();
    }

    var eventAdmins = new[] { admins[(month * 2) % admins.Count], admins[(month * 2 + 1) % admins.Count] };
    foreach (var admin in eventAdmins)
    {
        if (!await db.TblAdminEvents.AnyAsync(x => x.EventId == eventEntity.Id && x.AdminUserId == admin.Id))
            db.TblAdminEvents.Add(new TblAdminEvent { EventId = eventEntity.Id, AdminUserId = admin.Id, AssignedAt = monthStart.AddDays(2), AssignedBy = superAdmin.Id, IsActive = true, DeleteFlag = false });
    }

    var projects = new List<TblProject>();
    for (var typeIndex = 0; typeIndex < projectTypes.Count; typeIndex++)
    {
        var projectName = typeIndex switch
        {
            0 => $"Smart Farming Network {monthStart:MMM yyyy}",
            1 => $"Community Learning Platform {monthStart:MMM yyyy}",
            _ => $"Clean Township Initiative {monthStart:MMM yyyy}"
        };
        var project = await db.TblProjects.FirstOrDefaultAsync(x => x.EventId == eventEntity.Id && x.Name == projectName);
        if (project == null)
        {
            project = new TblProject { EventId = eventEntity.Id, ProjectTypeId = projectTypes[typeIndex].Id, Name = projectName, Description = "Myanmar community-focused project for annual voting records.", VoteCounts = 0, IsActive = true, DeleteFlag = false, CreatedAt = monthStart.AddDays(4), CreatedBy = eventAdmins[0].Id };
            db.TblProjects.Add(project);
        }
        projects.Add(project);
    }
    await db.SaveChangesAsync();

    var assignedVoters = Enumerable.Range(0, 40).Select(i => voters[(month * 17 + i) % voters.Count]).ToList();
    foreach (var voter in assignedVoters)
    {
        var userEvent = await db.TblUserEvents.FirstOrDefaultAsync(x => x.EventId == eventEntity.Id && x.UserId == voter.Id);
        if (userEvent == null)
        {
            userEvent = new TblUserEvent { UserId = voter.Id, EventId = eventEntity.Id, EventPasswordHash = hasher.HashPassword(voter, password), AssignedAt = monthStart.AddDays(5), AssignedBy = eventAdmins[0].Id, PasswordGeneratedAt = monthStart.AddDays(5), PasswordExpiresAt = eventEntity.EventEndAt.AddDays(7), AccessStatus = "valid", IsActive = true, DeleteFlag = false };
            db.TblUserEvents.Add(userEvent);
            await db.SaveChangesAsync();
        }

        for (var typeIndex = 0; typeIndex < 2; typeIndex++)
        {
            var projectType = projectTypes[typeIndex];
            var limit = await db.TblVotingLimits.FirstOrDefaultAsync(x => x.UserEventId == userEvent.Id && x.ProjectTypeId == projectType.Id);
            if (limit == null)
            {
                limit = new TblVotingLimit { UserEventId = userEvent.Id, ProjectTypeId = projectType.Id, VoteLimit = 1, VoteUsed = 0, CreatedAt = monthStart.AddDays(6), CreatedBy = eventAdmins[0].Id, DeleteFlag = false };
                db.TblVotingLimits.Add(limit);
                await db.SaveChangesAsync();
            }
            var project = projects[typeIndex];
            if (!await db.TblVotes.AnyAsync(x => x.UserEventId == userEvent.Id && x.ProjectId == project.Id))
            {
                db.TblVotes.Add(new TblVote { UserEventId = userEvent.Id, ProjectId = project.Id, VotedAt = votingStart.AddHours((voter.Id + typeIndex) % 72) });
                limit.VoteUsed = 1;
                project.VoteCounts++;
            }
        }
    }
    await db.SaveChangesAsync();

    var winner = projects.OrderByDescending(x => x.VoteCounts).First();
    if (!await db.TblResults.AnyAsync(x => x.EventId == eventEntity.Id && x.ProjectId == winner.Id && x.Version == 1))
        db.TblResults.Add(new TblResult { EventId = eventEntity.Id, ProjectId = winner.Id, Status = "first_prize", Version = 1, ReviewedAt = votingEnd.AddHours(12), ReviewedBy = eventAdmins[0].Id, PublishedAt = votingEnd.AddDays(1), PublishedBy = superAdmin.Id, CreatedAt = votingEnd.AddHours(6), CreatedBy = eventAdmins[0].Id, DeleteFlag = false });

    await db.SaveChangesAsync();
}

var report = new StringBuilder();
report.AppendLine("VOTING SYSTEM USER CREDENTIALS");
report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
report.AppendLine("Password for every account: Thanks!12@");
report.AppendLine("Counts: 5 SuperAdmin, 20 Admin, 200 User");
report.AppendLine(new string('=', 92));
report.AppendLine("No. | Role       | Name                         | Email");
report.AppendLine(new string('-', 92));
for (var index = 0; index < accounts.Count; index++)
    report.AppendLine($"{index + 1,3} | {accounts[index].Role,-10} | {accounts[index].Name,-28} | {accounts[index].Email}");
await File.WriteAllTextAsync(outputPath, report.ToString(), new UTF8Encoding(false));

Console.WriteLine($"Seed complete: {accounts.Count} accounts, 12 events, 36 projects.");
Console.WriteLine($"Credentials: {outputPath}");

static string Normalize(string value) => new(value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
internal sealed record AccountSeed(string Name, string Email, string Role);
