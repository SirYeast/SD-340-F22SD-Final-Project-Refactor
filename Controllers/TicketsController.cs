using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context; // to be removed
        private readonly TicketBusinessLogic _ticketBusinessLogic;

        public TicketsController(
            ApplicationDbContext context,
            TicketBusinessLogic ticketBusinessLogic)
        {
            _context = context;
            _ticketBusinessLogic = ticketBusinessLogic;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            return View(await _ticketBusinessLogic.GetAllTicketsAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                return View(await _ticketBusinessLogic.GetTicketDetails(id));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // GET: Tickets/Create
        [Authorize(Roles = "ProjectManager")]
        public IActionResult Create(int projId) // will be moved to ProjectBusinessLogic
        {
            Project currProject = _context.Projects.Include(p => p.AssignedTo).ThenInclude(at => at.ApplicationUser).FirstOrDefault(p => p.Id == projId);

            return View(new ItemWithUsersViewModel<Ticket>(new Ticket()
            {
                ProjectId = projId,
                Project = currProject,
            }, currProject.AssignedTo.Select(u => u.ApplicationUser)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create(ItemWithUsersViewModel<Ticket> vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _ticketBusinessLogic.CreateTicketAsync(vm.Item);
                    return RedirectToAction("Index", "Projects", new { area = "" });
                }
                catch (Exception ex)
                {
                    return Problem(ex.Message);
                }
            }
            return View(vm.Item);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                return View(await _ticketBusinessLogic.GetTicketForEditingAsync(id));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id, ItemWithUsersViewModel<Ticket> vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _ticketBusinessLogic.EditTicketAsync(id, vm.Item);
                    return RedirectToAction(nameof(Edit), new { id = vm.Item.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_ticketBusinessLogic.TicketExists(vm.Item.Id))
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
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "ProjectManager")]
        public IActionResult Delete(int? id)
        {
            try
            {
                return View(_ticketBusinessLogic.GetTicketForDeletion(id));
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public IActionResult DeleteConfirmed(int id)
        {
            _ticketBusinessLogic.DeleteTicket(id);
            return RedirectToAction("Index", "Projects");
        }

        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> RemoveAssignedUser(string id, int ticketId)
        {
            if (id == null)
            {
                return NotFound();
            }
            Ticket currTicket = await _context.Tickets.Include(t => t.Owner).FirstAsync(t => t.Id == ticketId);
            ApplicationUser currUser = await _context.Users.FirstAsync(u => u.Id == id);
            //To be fixed ASAP
            currTicket.Owner = currUser;
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = ticketId });
        }

        [HttpPost]
        public async Task<IActionResult> CommentTask(int TaskId, string? TaskText)
        {
            if (TaskId != null || TaskText != null)
            {
                try
                {
                    Comment newComment = new Comment();
                    string userName = User.Identity.Name;
                    ApplicationUser user = _context.Users.First(u => u.UserName == userName);
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == TaskId);

                    newComment.CreatedBy = user;
                    newComment.Description = TaskText;
                    newComment.Ticket = ticket;
                    user.Comments.Add(newComment);
                    _context.Comments.Add(newComment);
                    ticket.Comments.Add(newComment);

                    int Id = TaskId;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { Id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateHrs(int id, int hrs)
        {
            if (id != null || hrs != null)
            {
                try
                {
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
                    ticket.RequiredHours = hrs;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddToWatchers(int id)
        {
            if (id != null)
            {
                try
                {
                    TicketWatcher newTickWatch = new TicketWatcher();
                    string userName = User.Identity.Name;
                    ApplicationUser user = _context.Users.First(u => u.UserName == userName);
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);

                    newTickWatch.Ticket = ticket;
                    newTickWatch.Watcher = user;
                    user.TicketWatching.Add(newTickWatch);
                    ticket.TicketWatchers.Add(newTickWatch);
                    _context.Add(newTickWatch);

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UnWatch(int id)
        {
            if (id != null)
            {
                try
                {

                    string userName = User.Identity.Name;
                    ApplicationUser user = _context.Users.First(u => u.UserName == userName);
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
                    TicketWatcher currTickWatch = await _context.TicketWatchers.FirstAsync(tw => tw.Ticket.Equals(ticket) && tw.Watcher.Equals(user));
                    _context.TicketWatchers.Remove(currTickWatch);
                    ticket.TicketWatchers.Remove(currTickWatch);
                    user.TicketWatching.Remove(currTickWatch);

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            if (id != null)
            {
                try
                {
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
                    ticket.Completed = true;

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UnMarkAsCompleted(int id)
        {
            if (id != null)
            {
                try
                {
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
                    ticket.Completed = false;

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }
    }
}