using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using System.Net.Sockets;
using System.Security.Claims;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class TicketBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Ticket> _ticketRepo;
        private readonly IRepository<Project> _projectRepo;
        private readonly IRepository<Comment> _commentRepo;
        private readonly IRepository<UserProject> _userProjectRepo;
        private readonly IRepository<TicketWatcher> _ticketWatcherRepo;

        public TicketBusinessLogic(
            UserManager<ApplicationUser> userManager,
            IRepository<Ticket> ticketRepo,
            IRepository<Project> projectRepo,
            IRepository<Comment> commentRepo,
            IRepository<UserProject> userProjectRepo,
            IRepository<TicketWatcher> ticketWatcherRepo)
        {
            _userManager = userManager;
            _ticketRepo = ticketRepo;
            _projectRepo = projectRepo;
            _commentRepo = commentRepo;
            _userProjectRepo = userProjectRepo;
            _ticketWatcherRepo = ticketWatcherRepo;
        }

        private async Task<IEnumerable<ApplicationUser>> GetProjectAssignedUsers(int projectId)
        {
            List<ApplicationUser> assignedUsers = new();

            foreach (UserProject userProject in _userProjectRepo.GetAll().Where(u => u.ProjectId == projectId))
            {
                assignedUsers.Add(await _userManager.FindByIdAsync(userProject.ApplicationUserId));
            }

            return assignedUsers;
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            ICollection<Ticket> tickets = _ticketRepo.GetAll();

            foreach (Ticket ticket in tickets)
            {
                ticket.Project = _projectRepo.Get(ticket.ProjectId);
                ticket.Owner = await _userManager.FindByIdAsync(ticket.OwnerId);
            }

            return tickets;
        }

        public async Task<ItemWithUsersViewModel<Ticket>> GetTicketDetailsAsync(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            Ticket? ticket = _ticketRepo.Get(id)
                ?? throw new NullReferenceException("Cannot get details of null ticket");

            ticket.Owner = await _userManager.FindByIdAsync(ticket.OwnerId);
            ticket.Project = _projectRepo.Get(ticket.ProjectId);

            ticket.TicketWatchers = _ticketWatcherRepo.GetAll().Where(t => t.TicketId == id).ToList();

            foreach (TicketWatcher ticketWatcher in ticket.TicketWatchers)
            {
                ticketWatcher.Watcher = await _userManager.FindByIdAsync(ticketWatcher.WatcherId);
            }

            ticket.Comments = _commentRepo.GetAll().Where(c => c.TicketId == id).ToList();

            foreach (Comment comment in ticket.Comments)
            {
                comment.CreatedBy = await _userManager.FindByIdAsync(comment.CreatedById);
            }

            return new ItemWithUsersViewModel<Ticket>(ticket, await GetProjectAssignedUsers(ticket.ProjectId
                ?? throw new NullReferenceException("Cannot get assigned users of null project")));
        }

        public async Task<ItemWithUsersViewModel<Ticket>> GetTicketForCreationAsync(int? projectId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));

            Project? project = _projectRepo.Get(projectId) 
                ?? throw new NullReferenceException("Could not get ticket for creation because project does not exist");

            return new ItemWithUsersViewModel<Ticket>(new Ticket()
            {
                ProjectId = projectId,
                Project = project,
            }, await GetProjectAssignedUsers(project.Id));
        }

        public async Task CreateTicketAsync(Ticket ticket)
        {
            ticket.Owner = await _userManager.FindByIdAsync(ticket.OwnerId)
                ?? throw new NullReferenceException("Cannot create ticket without valid user");

            ticket.Project = _projectRepo.Get(ticket.ProjectId)
                ?? throw new NullReferenceException("Cannot create ticket without valid project");

            _ticketRepo.Create(ticket);
        }

        public async Task<ItemWithUsersViewModel<Ticket>> GetTicketForEditingAsync(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            Ticket? ticket = _ticketRepo.Get(id)
                ?? throw new NullReferenceException("Could not get valid ticket for editing");

            ticket.Owner = await _userManager.FindByIdAsync(ticket.OwnerId);

            return new ItemWithUsersViewModel<Ticket>(ticket, await GetProjectAssignedUsers(ticket.ProjectId
                ?? throw new NullReferenceException("Cannot get assigned users of null project")));
        }

        public void EditTicket(int id, Ticket ticket)
        {
            if (id != ticket.Id)
                throw new ArgumentException("Ticket ids do not match");

            if (!_userManager.Users.Any(u => u.Id == ticket.OwnerId))
                throw new ArgumentException("Ticket is owned by user that does not exist");

            _ticketRepo.Update(ticket);
        }

        public Ticket GetTicketForDeletion(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return _ticketRepo.Get(id)
                ?? throw new NullReferenceException("Could not get valid ticket for deletion");
        }

        public void DeleteTicket(int id)
        {
            Ticket? ticket = _ticketRepo.Get(id);

            if (ticket != null)
                _ticketRepo.Delete(ticket);
        }

        public async Task CommentTicketAsync(int? id, string? text, ClaimsPrincipal claimsPrincipal)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (text == null)
                throw new ArgumentNullException(nameof(text));

            Ticket? ticket = _ticketRepo.Get(id)
                ?? throw new NullReferenceException("Cannot comment on null ticket");

            ApplicationUser? user = await _userManager.GetUserAsync(claimsPrincipal)
                ?? throw new NullReferenceException("Null user cannot comment on ticket");

            _commentRepo.Create(new Comment()
            {
                TicketId = ticket.Id,
                CreatedById = user.Id,
                Description = text,
            });
        }

        public void UpdateHours(int? id, int hours)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            Ticket? ticket = _ticketRepo.Get(id)
                ?? throw new NullReferenceException("Cannot update hours on null ticket");

            ticket.RequiredHours = hours;
            _ticketRepo.Update(ticket);
        }

        public async Task WatchTicketAsync(int? id, ClaimsPrincipal claimsPrincipal)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            Ticket? ticket = _ticketRepo.Get(id)
                ?? throw new NullReferenceException("Cannot watch a null ticket");

            ApplicationUser? user = await _userManager.GetUserAsync(claimsPrincipal)
                ?? throw new NullReferenceException("Null user cannot watch ticket");

            _ticketWatcherRepo.Create(new TicketWatcher()
            {
                TicketId = ticket.Id,
                WatcherId = user.Id
            });
        }

        public async Task UnwatchTicketAsync(int? id, ClaimsPrincipal claimsPrincipal)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            Ticket? ticket = _ticketRepo.Get(id)
                ?? throw new NullReferenceException("Cannot unwatch a null ticket");

            ApplicationUser? user = await _userManager.GetUserAsync(claimsPrincipal)
                ?? throw new NullReferenceException("Null user cannot unwatch ticket");

            TicketWatcher? currTickWatch = _ticketWatcherRepo.GetAll().FirstOrDefault(t => t.TicketId == id && t.WatcherId == user.Id);

            if (currTickWatch != null)
                _ticketWatcherRepo.Delete(currTickWatch);
        }

        public void UpdateTicketCompleted(int? id, bool isCompleted)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            Ticket? ticket = _ticketRepo.Get(id)
                ?? throw new NullReferenceException("Cannot mark null ticket as completed or not");

            ticket.Completed = isCompleted;
            _ticketRepo.Update(ticket);
        }

        public bool TicketExists(int id)
        {
            return _ticketRepo.Exists(id);
        }
    }
}
