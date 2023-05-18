using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class AdminBusinessLogic
    {
        private UserManager<ApplicationUser> _userManager;
        public AdminBusinessLogic(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ProjectManagersAndDevelopersViewModels> ProjectManagersAndDevelopersViewModels()
        {
            ProjectManagersAndDevelopersViewModels vm = new ProjectManagersAndDevelopersViewModels();

            List<ApplicationUser> pmUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("ProjectManager");
            List<ApplicationUser> devUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Developer");
            List<ApplicationUser> allUsers = _userManager.Users.ToList();



            vm.pms = pmUsers;
            vm.devs = devUsers;
            vm.allUsers = allUsers;
            return vm;
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            List<ApplicationUser> allUsers = await _userManager.Users.ToListAsync();
            return allUsers;

        }
        public async Task<ApplicationUser> GetUserByIdAsync(string? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            string userId = id.ToString();
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            return user;
        }
        public async Task<List<string>> GetRolesAsync(string role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            List<ApplicationUser> users = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync(role);
            List<string> userNames = users.Select(u => u.UserName).ToList();

            return userNames;
        }


        public async Task<List<ApplicationUser>> AddToRoleAsync(ApplicationUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException(nameof(role));
            }

            await _userManager.AddToRoleAsync(user, role);

            List<ApplicationUser> newUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync(role);

            return newUsers;
        }

        public async Task<List<ApplicationUser>> RemoveFromRoleAsync(ApplicationUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException(nameof(role));
            }

            await _userManager.RemoveFromRoleAsync(user, role);

            List<ApplicationUser> newUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync(role);

            return newUsers;
        }

        public async Task<List<SelectListItem>> ReassignRoleAsync()
        {
            List<ApplicationUser> allUsers = _userManager.Users.ToList();

            List<SelectListItem> users = allUsers.Select(u => new SelectListItem { Text = u.UserName, Value = u.Id.ToString() }).ToList();

            return users;
        }

    }
}
