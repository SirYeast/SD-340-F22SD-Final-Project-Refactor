using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using X.PagedList;
using X.PagedList.Mvc;


namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "ProjectManager, Developer")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _users;
        private readonly ProjectsBusinessLogic _projectBusinessLogic;


        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> users, ProjectsBusinessLogic projectsBusinessLogic)
        {
            _projectBusinessLogic = projectsBusinessLogic;
            _context = context;
            _users = users;
        }
        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index(string? sortOrder, int? page, bool? sort, string? userId)
        {
            List<Project> SortedProjs = new List<Project>();
            List<ApplicationUser> allUsers = (List<ApplicationUser>)await _users.GetUsersInRoleAsync("Developer");

            List<SelectListItem> users = new List<SelectListItem>();
            allUsers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });
            ViewBag.Users = users;
            switch (sortOrder)
            {
                case "Priority":
                    if (sort == true)
                    {
                        SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.ApplicationUser)
                        .Include(p => p.Tickets.OrderByDescending(t => t.TicketPriority))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    }
                    else
                    {
                        SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.ApplicationUser)
                        .Include(p => p.Tickets.OrderBy(t => t.TicketPriority))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    }

                    break;
                case "RequiredHrs":
                    if (sort == true)
                    {
                        SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.ApplicationUser)
                        .Include(p => p.Tickets.OrderByDescending(t => t.RequiredHours))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    }
                    else
                    {
                        SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.ApplicationUser)
                        .Include(p => p.Tickets.OrderBy(t => t.RequiredHours))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    }

                    break;
                case "Completed":
                    SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.ApplicationUser)
                        .Include(p => p.Tickets.Where(t => t.Completed == true))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    break;
                default:
                    if (userId != null)
                    {
                        SortedProjs =
                        await _context.Projects
                        .OrderBy(p => p.ProjectName)
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.ApplicationUser)
                        .Include(p => p.Tickets.Where(t => t.Owner.Id.Equals(userId)))
                        .ThenInclude(t => t.Owner)
                        .Include(p => p.Tickets).ThenInclude(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher)
                        .ToListAsync();
                    }
                    else
                    {
                        SortedProjs =
                        await _context.Projects
                        .OrderBy(p => p.ProjectName)
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.ApplicationUser)
                        .Include(p => p.Tickets)
                        .ThenInclude(t => t.Owner)
                        .Include(p => p.Tickets).ThenInclude(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher)
                        .ToListAsync();
                    }

                    break;
            }
            //check if User is PM or Develoer
            var LogedUserName = User.Identity.Name;  // logined user name
            var user = _context.Users.FirstOrDefault(u => u.UserName == LogedUserName);
            var rolenames = await _users.GetRolesAsync(user);
            var AssinedProject = new List<Project>();
            // geting assined project
            if (rolenames.Contains("Developer"))
            {
                AssinedProject = SortedProjs.Where(p => p.AssignedTo.Select(projectUser => projectUser.ApplicationUserId).Contains(user.Id)).ToList();
            }
            else
            {
                AssinedProject = SortedProjs;
            }
            X.PagedList.IPagedList<Project> projList = AssinedProject.ToPagedList(page ?? 1, 3);
            return View(projList);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int id)
        {
            
            try {
                return View(_projectBusinessLogic.GetDetailsById(id)); 
            } 
            catch (Exception ex){
                return Problem(ex.Message);
            }
        }

        public async Task<IActionResult> RemoveAssignedUser(string id, int projId)
        {
            try
            {
                await _projectBusinessLogic.RemoveUserFromProjectAsync(id, projId);
                return RedirectToAction("Edit", new { id = projId });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // GET: Projects/Create
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> CreateAsync()
        {
            try
            {
                ViewBag.Users = await _projectBusinessLogic.GetUsersForCreateAsync();
                return View();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,ProjectName")] Project project, List<string> userIds)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _projectBusinessLogic.CreateProjectAsync(userIds, project, User);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    return Problem($"Could not create {ex.Message}");
                }
            }
            return View();
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                return View(await _projectBusinessLogic.GetProjectForEdit(id));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id, List<string> userids, ItemWithUsersViewModel<Project> vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _projectBusinessLogic.EditProject(id, userids, vm.Item);
                    return RedirectToAction(nameof(Edit), new { id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_projectBusinessLogic.ProjectExists(vm.Item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    return Problem(ex.Message);
                }
            }
            return View(vm.Item);
            //Insert Business Logic
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                Project project = _projectBusinessLogic.GetProjectForDelete(id);
                return View(project);
            }
            catch(Exception ex) 
            { 
                return Problem(ex.Message);
            }
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _projectBusinessLogic.DeleteProject(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
