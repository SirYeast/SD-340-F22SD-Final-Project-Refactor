using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

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

        public async Task<ItemWithUsersViewModel<Ticket>> GetTicketDetails(int? id)
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

            return new ItemWithUsersViewModel<Ticket>(ticket, _userProjectRepo.GetAll().Where(u => u.ProjectId == ticket.ProjectId).Select(u => u.ApplicationUser));
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
            Ticket? ticket = _ticketRepo.Get(id)
                ?? throw new NullReferenceException("Could not not get valid ticket for editing");

            ticket.Owner = await _userManager.FindByIdAsync(ticket.OwnerId);

            return new ItemWithUsersViewModel<Ticket>(ticket,
                _userProjectRepo.GetAll().Where(u => u.ProjectId == ticket.ProjectId).Select(u => u.ApplicationUser));
        }

        public async Task EditTicketAsync(int id, Ticket ticket)
        {
            if (id != ticket.Id)
                throw new ArgumentException("Ticket ids do not match");

            if (!await _userManager.Users.AnyAsync(u => u.Id == ticket.OwnerId))
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

        public bool TicketExists(int id)
        {
            return _ticketRepo.Exists(id);
        }
    }
}
