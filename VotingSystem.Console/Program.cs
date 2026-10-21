using VotingSystem.Console.Features;
using VotingSystem.Console.Features.Projects;
using VotingSystem.Console.Features.Users;
using VotingSystem.Console.Features.Votes;
using VotingSystem.Console.MemoryData;
using VotingSystem.Console.Models;

IAuthService authService = new AuthService();
IUserService userService = new UserService();
IProjectService projectService = new ProjectService();
IVotingService votingService = new VotingService();

RunMainMenu();

void RunMainMenu()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("===== Voting System =====");
        Console.WriteLine("1. Admin Login");
        Console.WriteLine("2. User Login");
        Console.WriteLine("3. Exit");
        Console.Write("Please enter your choice (1-3): ");

        string? input = Console.ReadLine()?.Trim();

        switch (input)
        {
            case "1":
                HandleAdminLogin();
                break;
            case "2":
                HandleUserLogin();
                break;
            case "3":
                Console.WriteLine();
                Console.WriteLine("Exiting the application. Thank you!");
                return;
            default:
                Console.WriteLine("Invalid option. Please enter 1, 2, or 3.");
                break;
        }
    }
}

#region Authentication Flows

void HandleAdminLogin()
{
    Console.WriteLine();
    Console.WriteLine("--- Admin Login ---");
    Console.Write("Enter Username: ");
    string? username = Console.ReadLine()?.Trim();
    Console.Write("Enter Password: ");
    string? password = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
    {
        Console.WriteLine("Error: Username and password cannot be empty.");
        return;
    }

    bool success = authService.Login(username, password);
    if (!success)
    {
        Console.WriteLine("Error: Invalid username or password.");
        return;
    }

    User? user = userService.GetUser(username);
    if (user == null || user.Role != Enums.UserRoles.Admin)
    {
        Console.WriteLine("Error: Access denied. This account does not have Admin privileges.");
        return;
    }

    Console.WriteLine($"Login successful! Welcome, Admin {user.Username}.");
    RunAdminMenu(user);
}

void HandleUserLogin()
{
    Console.WriteLine();
    Console.WriteLine("--- User Login ---");
    Console.Write("Enter Username: ");
    string? username = Console.ReadLine()?.Trim();
    Console.Write("Enter Password: ");
    string? password = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
    {
        Console.WriteLine("Error: Username and password cannot be empty.");
        return;
    }

    bool success = authService.Login(username, password);
    if (!success)
    {
        Console.WriteLine("Error: Invalid username or password.");
        return;
    }

    User? user = userService.GetUser(username);
    if (user == null || user.Role != Enums.UserRoles.User)
    {
        Console.WriteLine("Error: Access denied. Admin accounts cannot log in as regular voters.");
        return;
    }

    Console.WriteLine($"Login successful! Welcome, {user.Username}.");
    RunUserMenu(user.Username);
}

#endregion

#region Admin Menus & Actions

void RunAdminMenu(User adminUser)
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("===== Admin Menu =====");
        Console.WriteLine("1. Create Project");
        Console.WriteLine("2. Project Management");
        Console.WriteLine("3. Create User");
        Console.WriteLine("4. User Management");
        Console.WriteLine("5. View Project Voting Details");
        Console.WriteLine("6. Logout");
        Console.Write("Please enter your choice (1-6): ");

        string? input = Console.ReadLine()?.Trim();

        switch (input)
        {
            case "1":
                CreateProjectAction();
                break;
            case "2":
                RunProjectManagementMenu();
                break;
            case "3":
                CreateUserAction();
                break;
            case "4":
                RunUserManagementMenu();
                break;
            case "5":
                ViewProjectVotingDetailsAction();
                break;
            case "6":
                Console.WriteLine("Logged out successfully.");
                return;
            default:
                Console.WriteLine("Invalid option. Please enter a number between 1 and 6.");
                break;
        }
    }
}

void CreateProjectAction()
{
    Console.WriteLine();
    Console.WriteLine("--- Create Project ---");
    Console.Write("Enter Project Name: ");
    string? name = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Error: Project name cannot be empty.");
        return;
    }

    Console.Write("Enter Project Description: ");
    string? description = Console.ReadLine()?.Trim() ?? string.Empty;

    Project project = projectService.CreateProject(name, description);
    Data.MaxProjId = project.Id;

    Console.WriteLine($"Success: Project created successfully! [ID: {project.Id}, Name: {project.Name}]");
}

void RunProjectManagementMenu()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("===== Project Management =====");
        Console.WriteLine("1. View All Projects");
        Console.WriteLine("2. View Project by ID");
        Console.WriteLine("3. Update Project");
        Console.WriteLine("4. Delete Project");
        Console.WriteLine("5. Back to Admin Menu");
        Console.Write("Please enter your choice (1-5): ");

        string? input = Console.ReadLine()?.Trim();

        switch (input)
        {
            case "1":
                ViewAllProjectsAction();
                break;
            case "2":
                ViewProjectByIdAction();
                break;
            case "3":
                UpdateProjectAction();
                break;
            case "4":
                DeleteProjectAction();
                break;
            case "5":
                return;
            default:
                Console.WriteLine("Invalid option. Please enter a number between 1 and 5.");
                break;
        }
    }
}

void ViewAllProjectsAction()
{
    Console.WriteLine();
    Console.WriteLine("--- All Projects ---");
    var projects = projectService.GetProjects();
    if (projects == null || projects.Count == 0)
    {
        Console.WriteLine("No projects found.");
        return;
    }

    foreach (var project in projects)
    {
        Console.WriteLine($"ID: {project.Id} | Name: {project.Name} | Description: {project.Description}");
    }
}

void ViewProjectByIdAction()
{
    Console.WriteLine();
    Console.Write("Enter Project ID: ");
    if (!int.TryParse(Console.ReadLine(), out int projectId))
    {
        Console.WriteLine("Error: Invalid Project ID format.");
        return;
    }

    Project? project = projectService.GetProject(projectId);
    if (project == null)
    {
        Console.WriteLine($"Error: Project with ID {projectId} not found.");
        return;
    }

    Console.WriteLine($"ID: {project.Id}");
    Console.WriteLine($"Name: {project.Name}");
    Console.WriteLine($"Description: {project.Description}");
}

void UpdateProjectAction()
{
    Console.WriteLine();
    Console.Write("Enter Project ID to update: ");
    if (!int.TryParse(Console.ReadLine(), out int projectId))
    {
        Console.WriteLine("Error: Invalid Project ID format.");
        return;
    }

    Project? existing = projectService.GetProject(projectId);
    if (existing == null)
    {
        Console.WriteLine($"Error: Project with ID {projectId} not found.");
        return;
    }

    Console.Write($"Enter New Name (Current: {existing.Name}): ");
    string? newName = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(newName))
    {
        Console.WriteLine("Error: Project name cannot be empty.");
        return;
    }

    Console.Write($"Enter New Description (Current: {existing.Description}): ");
    string? newDescription = Console.ReadLine()?.Trim() ?? string.Empty;

    Project? updated = projectService.UpdateProject(projectId, newName, newDescription);
    if (updated != null)
    {
        Console.WriteLine($"Success: Project ID {projectId} updated successfully.");
    }
    else
    {
        Console.WriteLine("Error: Failed to update project.");
    }
}

void DeleteProjectAction()
{
    Console.WriteLine();
    Console.Write("Enter Project ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out int projectId))
    {
        Console.WriteLine("Error: Invalid Project ID format.");
        return;
    }

    bool deleted = projectService.DeleteProject(projectId);
    if (deleted)
    {
        Console.WriteLine($"Success: Project with ID {projectId} deleted successfully.");
    }
    else
    {
        Console.WriteLine($"Error: Project with ID {projectId} not found.");
    }
}

void CreateUserAction()
{
    Console.WriteLine();
    Console.WriteLine("--- Create User ---");
    Console.Write("Enter Username: ");
    string? username = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(username))
    {
        Console.WriteLine("Error: Username cannot be empty.");
        return;
    }

    if (userService.GetUser(username) != null)
    {
        Console.WriteLine($"Error: User with username '{username}' already exists.");
        return;
    }

    Console.Write("Enter Password: ");
    string? password = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(password))
    {
        Console.WriteLine("Error: Password cannot be empty.");
        return;
    }

    Console.WriteLine("Select User Role:");
    Console.WriteLine("1. Admin");
    Console.WriteLine("2. User");
    Console.Write("Enter choice (1-2): ");
    string? roleChoice = Console.ReadLine()?.Trim();

    Enums.UserRoles role;
    if (roleChoice == "1")
    {
        role = Enums.UserRoles.Admin;
    }
    else if (roleChoice == "2")
    {
        role = Enums.UserRoles.User;
    }
    else
    {
        Console.WriteLine("Error: Invalid role selection.");
        return;
    }

    User newUser = userService.CreateUser(username, password, role);
    Data.MaxUserId = newUser.Id;

    if (role == Enums.UserRoles.Admin)
    {
        votingService.UpdateUserVoteCount(newUser.Username, 0);
    }

    Console.WriteLine($"Success: User '{newUser.Username}' created successfully with role {newUser.Role} [ID: {newUser.Id}].");
}

void RunUserManagementMenu()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("===== User Management =====");
        Console.WriteLine("1. View All Users");
        Console.WriteLine("2. View User by Username");
        Console.WriteLine("3. Update User");
        Console.WriteLine("4. Delete User");
        Console.WriteLine("5. Back to Admin Menu");
        Console.Write("Please enter your choice (1-5): ");

        string? input = Console.ReadLine()?.Trim();

        switch (input)
        {
            case "1":
                ViewAllUsersAction();
                break;
            case "2":
                ViewUserByUsernameAction();
                break;
            case "3":
                UpdateUserAction();
                break;
            case "4":
                DeleteUserAction();
                break;
            case "5":
                return;
            default:
                Console.WriteLine("Invalid option. Please enter a number between 1 and 5.");
                break;
        }
    }
}

void ViewAllUsersAction()
{
    Console.WriteLine();
    Console.WriteLine("--- All Users ---");
    var users = userService.GetUsers();
    if (users == null || users.Count == 0)
    {
        Console.WriteLine("No users found.");
        return;
    }

    foreach (var user in users)
    {
        Console.WriteLine($"ID: {user.Id} | Username: {user.Username} | Role: {user.Role} | Remaining Votes: {user.votingCount}");
    }
}

void ViewUserByUsernameAction()
{
    Console.WriteLine();
    Console.Write("Enter Username: ");
    string? username = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(username))
    {
        Console.WriteLine("Error: Username cannot be empty.");
        return;
    }

    User? user = userService.GetUser(username);
    if (user == null)
    {
        Console.WriteLine($"Error: User '{username}' not found.");
        return;
    }

    Console.WriteLine($"ID: {user.Id}");
    Console.WriteLine($"Username: {user.Username}");
    Console.WriteLine($"Role: {user.Role}");
    Console.WriteLine($"Remaining Votes: {user.votingCount}");
}

void UpdateUserAction()
{
    Console.WriteLine();
    Console.Write("Enter Username of user to update: ");
    string? username = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(username))
    {
        Console.WriteLine("Error: Username cannot be empty.");
        return;
    }

    User? existing = userService.GetUser(username);
    if (existing == null)
    {
        Console.WriteLine($"Error: User '{username}' not found.");
        return;
    }

    Console.Write("Enter New Password: ");
    string? newPassword = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(newPassword))
    {
        Console.WriteLine("Error: Password cannot be empty.");
        return;
    }

    Console.WriteLine("Select New Role:");
    Console.WriteLine("1. Admin");
    Console.WriteLine("2. User");
    Console.Write("Enter choice (1-2): ");
    string? roleChoice = Console.ReadLine()?.Trim();

    Enums.UserRoles newRole;
    if (roleChoice == "1")
    {
        newRole = Enums.UserRoles.Admin;
    }
    else if (roleChoice == "2")
    {
        newRole = Enums.UserRoles.User;
    }
    else
    {
        Console.WriteLine("Error: Invalid role selection.");
        return;
    }

    User? updated = userService.UpdateUser(username, newPassword, newRole);
    if (updated != null)
    {
        if (newRole == Enums.UserRoles.Admin)
        {
            votingService.UpdateUserVoteCount(updated.Username, 0);
        }
        Console.WriteLine($"Success: User '{username}' updated successfully.");
    }
    else
    {
        Console.WriteLine("Error: Failed to update user.");
    }
}

void DeleteUserAction()
{
    Console.WriteLine();
    Console.Write("Enter Username of user to delete: ");
    string? username = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(username))
    {
        Console.WriteLine("Error: Username cannot be empty.");
        return;
    }

    bool deleted = userService.DeleteUser(username);
    if (deleted)
    {
        Console.WriteLine($"Success: User '{username}' deleted successfully.");
    }
    else
    {
        Console.WriteLine($"Error: User '{username}' not found.");
    }
}

void ViewProjectVotingDetailsAction()
{
    Console.WriteLine();
    Console.WriteLine("===== Project Voting Details =====");
    var projects = projectService.GetProjects();
    if (projects == null || projects.Count == 0)
    {
        Console.WriteLine("No projects found.");
        return;
    }

    foreach (var project in projects)
    {
        Console.WriteLine($"{project.Name} - {project.VoteCount} votes");
    }
}

#endregion

#region User Menus & Actions

void RunUserMenu(string username)
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("===== User Menu =====");
        Console.WriteLine("1. View Project List");
        Console.WriteLine("2. Vote for a Project");
        Console.WriteLine("3. View My Remaining Voting Count");
        Console.WriteLine("4. Logout");
        Console.Write("Please enter your choice (1-4): ");

        string? input = Console.ReadLine()?.Trim();

        switch (input)
        {
            case "1":
                ViewProjectListForUserAction();
                break;
            case "2":
                VoteForProjectAction(username);
                break;
            case "3":
                ViewRemainingVotingCountAction(username);
                break;
            case "4":
                Console.WriteLine("Logged out successfully.");
                return;
            default:
                Console.WriteLine("Invalid option. Please enter a number between 1 and 4.");
                break;
        }
    }
}

void ViewProjectListForUserAction()
{
    Console.WriteLine();
    Console.WriteLine("--- Available Projects ---");
    var projects = projectService.GetProjects();
    if (projects == null || projects.Count == 0)
    {
        Console.WriteLine("No projects available.");
        return;
    }

    foreach (var project in projects)
    {
        Console.WriteLine($"{project.Id}. {project.Name}");
    }
}

void VoteForProjectAction(string username)
{
    Console.WriteLine();
    Console.WriteLine("--- Vote for a Project ---");
    var projects = projectService.GetProjects();
    if (projects == null || projects.Count == 0)
    {
        Console.WriteLine("No projects available to vote for.");
        return;
    }

    Console.WriteLine("Available Projects:");
    foreach (var project in projects)
    {
        Console.WriteLine($"{project.Id}. {project.Name}");
    }

    Console.Write("Enter the Project ID you want to vote for: ");
    if (!int.TryParse(Console.ReadLine(), out int projectId))
    {
        Console.WriteLine("Error: Invalid Project ID format.");
        return;
    }

    bool success = votingService.VoteProject(projectId, username);
    if (success)
    {
        Console.WriteLine("Success: Thank you! Your vote has been recorded successfully.");
    }
    else
    {
        Console.WriteLine("Error: Voting failed. You may have no remaining votes or the Project ID is invalid.");
    }
}

void ViewRemainingVotingCountAction(string username)
{
    Console.WriteLine();
    User? user = userService.GetUser(username);
    if (user != null)
    {
        Console.WriteLine($"Your remaining voting count: {user.votingCount}");
    }
    else
    {
        Console.WriteLine("Error: User information could not be retrieved.");
    }
}

#endregion
