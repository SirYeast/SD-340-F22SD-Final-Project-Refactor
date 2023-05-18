using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class ItemWithUsersViewModel<T>
    {
        public T Item { get; set; }
        public List<SelectListItem> Users { get; } = new();

        public ItemWithUsersViewModel() { }

        public ItemWithUsersViewModel(T item, IEnumerable<ApplicationUser> users)
        {
            Item = item;

            foreach (ApplicationUser user in users)
            {
                Users.Add(new SelectListItem(user.UserName, user.Id.ToString()));
            }
        }
    }
}
