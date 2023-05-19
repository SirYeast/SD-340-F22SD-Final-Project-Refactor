using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Experimental.ProjectCache;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class ProjectsBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Ticket> _ticketRepo;
        private readonly IRepository<Project> _projectRepo;
        private readonly IRepository<Comment> _commentRepo;
        private readonly IRepository<UserProject> _userProjectRepo;
        private readonly IRepository<TicketWatcher> _ticketWatcherRepo;

        public ProjectsBusinessLogic(
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

        //Details
        public Project GetDetailsById(int id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            Project project = _projectRepo.Get(id);
            if (project == null)
            {
                throw new NullReferenceException("Project Returned Null");
            }

            return project;
        }

        //Remove User From Project
        public async Task RemoveUserFromProject(string userId, int projectId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("User could not be found");
            }

            Project project = _projectRepo.Get(projectId);
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            UserProject? currUserProj = _userProjectRepo.GetAll().FirstOrDefault(up => up.ProjectId == project.Id && up.ApplicationUser.Id == userId);
            if(currUserProj == null)
            {
                throw new ArgumentNullException("Could not find UserProject");
            }

            _userProjectRepo.Delete(currUserProj);
        }
    }
}
