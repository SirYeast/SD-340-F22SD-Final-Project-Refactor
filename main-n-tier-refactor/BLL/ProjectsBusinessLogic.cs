using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using System.Security.Claims;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;
using NuGet.Versioning;
using Microsoft.Build.Construction;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Linq;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class ProjectsBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Ticket> _ticketRepo;
        private readonly IRepository<Project> _projectRepo;
        private readonly IRepository<UserProject> _userProjectRepo;

        public UserManager<ApplicationUser> Object { get; }
        public ProjectsRepository ProjectsRepository { get; }
        public UserProjectRepository UserProjectRepository { get; }

        public ProjectsBusinessLogic(
            UserManager<ApplicationUser> userManager,
            IRepository<Ticket> ticketRepo,
            IRepository<Project> projectRepo,
            IRepository<UserProject> userProjectRepo)
        {
            _ticketRepo= ticketRepo;
            _userManager = userManager;
            _projectRepo = projectRepo;
            _userProjectRepo = userProjectRepo;
        }

        public ProjectsBusinessLogic(UserManager<ApplicationUser> @object, ProjectsRepository projectsRepository, UserProjectRepository userProjectRepository)
        {
            Object = @object;
            ProjectsRepository = projectsRepository;
            UserProjectRepository = userProjectRepository;
        }

        //Index

        public async Task<IEnumerable<SelectListItem>> GetAllDevelopersAsync()
        {
            ICollection<ApplicationUser> allUsers = await _userManager.GetUsersInRoleAsync("Developer");

            List<SelectListItem> users = new List<SelectListItem>();

            foreach(ApplicationUser au in allUsers)
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            };

            return users;
        }

        public async Task<IPagedList<Project>> GetSortedProjects(string? sortOrder, int? page, bool? sort, string? userId, ClaimsPrincipal claimsPrincipal)
        {
            throw new NotImplementedException("Not Implemented");
        }
        //
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
        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("User could not be found");
            }

            Project project = _projectRepo.Get(projectId);
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            UserProject? currUserProj = _userProjectRepo.GetAll().FirstOrDefault(up => up.ProjectId == project.Id && up.ApplicationUser.Id == userId);
            if (currUserProj == null)
            {
                throw new ArgumentNullException("Could not find UserProject");
            }

            _userProjectRepo.Delete(currUserProj);
        }

        //Create Project

        //FIX SAVE ERROR
        public async Task CreateProjectAsync(List<string> userIds, Project project, ClaimsPrincipal claimsPrincipal)
        {
            ApplicationUser createdBy = await _userManager.GetUserAsync(claimsPrincipal);
            if (createdBy == null)
            {
                throw new ArgumentNullException("Could not find Current User");
            }

            userIds.ForEach((user) =>
            {
                ApplicationUser currUser = _userManager.Users.FirstOrDefault(u => u.Id == user);
                UserProject newUserProj = new UserProject();
                newUserProj.ApplicationUser = currUser;
                newUserProj.ApplicationUserId = currUser.Id;
                newUserProj.Project = project;
                project.AssignedTo.Add(newUserProj);
            });

            _projectRepo.Create(project);
        }

        //Edit
        public async Task<ItemWithUsersViewModel<Project>> GetProjectForEdit(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("Id null");
            }

            Project project = _projectRepo.Get(id);

            project.AssignedTo = _userProjectRepo.GetAll().Where(u=> u.ProjectId == project.Id).ToList();

            if (project == null)
            {
                throw new ArgumentException("Project Not Found");
            }

            IEnumerable<ApplicationUser> allDevs = await _userManager.GetUsersInRoleAsync("Developer");

            return new ItemWithUsersViewModel<Project>(project, allDevs);
        }

        public async Task EditProject(int id, List<string> userIds, Project project)
        {
            if (id != project.Id)
                throw new ArgumentException("Project ids do not match");

             userIds.ForEach(user => { 
                ApplicationUser currUser = _userManager.Users.FirstOrDefault(u => u.Id == user);

                UserProject newUserProj = new UserProject();
                newUserProj.ApplicationUser = currUser;
                newUserProj.ApplicationUserId = currUser.Id;
                newUserProj.Project = project;
                project.AssignedTo.Add(newUserProj);
            });

            _projectRepo.Update(project);
    }

        //Delete
        public Project GetProjectForDelete(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("Cannot Find Project With Id of NULL");
            }

            Project project = _projectRepo.Get(id);
            project.Tickets = _ticketRepo.GetAll().Where(t => t.ProjectId == id).ToList();
            if (project == null)
            {
                throw new ArgumentException("Project does not exist");
            }

            return project;
        }

        public void DeleteProject(int id)
        {
            if(_projectRepo.GetAll() == null)
            {
                throw new ArgumentNullException("Entity set 'ApplicationDbContext.Projects' is null.");
            }

            Project project = _projectRepo.Get(id);
            project.Tickets = _ticketRepo.GetAll().Where(t => t.ProjectId == id).ToList();


            if (project != null)
            {
                List<Ticket> tickets = project.Tickets.ToList();

                tickets.ForEach(ticket =>
                {
                    _ticketRepo.Delete(ticket);
                });

                List<UserProject> userProjects = _userProjectRepo.GetAll().Where(up => up.ProjectId == project.Id).ToList();
                userProjects.ForEach(userProj =>
                {
                    _userProjectRepo.Delete(userProj);
                });
            }
            _projectRepo.Delete(project);
        }

        public bool ProjectExists(int id)
        {
            return _projectRepo.Exists(id);
        }
    }
}
