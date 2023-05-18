using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AdminBusinessLogic _adminBusinessLogic;

        public AdminController(AdminBusinessLogic adminBusinessLogic)
        {
            _adminBusinessLogic = adminBusinessLogic;
        }
        public async Task<IActionResult> Index()
        {
            ProjectManagersAndDevelopersViewModels vm = await _adminBusinessLogic.ProjectManagersAndDevelopersViewModels();

            return View(vm);
        }

        public async Task<IActionResult> ReassignRoleAsync()
        {
            List<SelectListItem> allUsers = await _adminBusinessLogic.ReassignRoleAsync();

            ViewBag.Users = allUsers;

            return View(allUsers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReassignRole(string role, string userId)
        {

            ApplicationUser user = _users.Users.First(u => u.Id == userId);
            ICollection<string> roleUser = await _users.GetRolesAsync(user);
            if (roleUser.Count == 0)
            {
                await _users.AddToRoleAsync(user, role);
                return RedirectToAction("Index", "Admin", new { area = "" });
            } else
            {
                await _users.RemoveFromRoleAsync(user, roleUser.First());
                await _users.AddToRoleAsync(user, role);
                return RedirectToAction("Index", "Admin", new { area = "" });
            }
        }
    }
}

