using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly TicketBusinessLogic _ticketBusinessLogic;

        public TicketsController(TicketBusinessLogic ticketBusinessLogic)
        {
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
                return View(await _ticketBusinessLogic.GetTicketDetailsAsync(id));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // GET: Tickets/Create
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create(int projId)
        {
            try
            {
                return View(await _ticketBusinessLogic.GetTicketForCreationAsync(projId));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
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
                    _ticketBusinessLogic.EditTicket(id, vm.Item);
                    return RedirectToAction(nameof(Edit), new { id });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentTicket(int? id, string? text)
        {
            try
            {
                await _ticketBusinessLogic.CommentTicketAsync(id, text, User);
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateHrs(int? id, int hrs)
        {
            try
            {
                _ticketBusinessLogic.UpdateHours(id, hrs);
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        public async Task<IActionResult> AddToWatchers(int? id)
        {
            try
            {
                await _ticketBusinessLogic.WatchTicketAsync(id, User);
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        public async Task<IActionResult> UnWatch(int? id)
        {
            try
            {
                await _ticketBusinessLogic.UnwatchTicketAsync(id, User);
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        public IActionResult UpdatedCompleted(int? id, bool isCompleted)
        {
            try
            {
                _ticketBusinessLogic.UpdateTicketCompleted(id, isCompleted);
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}