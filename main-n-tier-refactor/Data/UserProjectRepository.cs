using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class UserProjectRepository : IRepository<UserProject>
    {
        private readonly ApplicationDbContext _context;

        public UserProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(UserProject entity)
        {
            _context.UserProjects.Add(entity);
            _context.SaveChanges();
        }

        public UserProject? Get(int? Id)
        {
            return _context.UserProjects.FirstOrDefault(u => u.Id == Id);
        }

        public ICollection<UserProject> GetAll()
        {
            return _context.UserProjects.ToList();
        }

        public void Update(UserProject entity)
        {
            _context.UserProjects.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(UserProject entity)
        {
            _context.UserProjects.Remove(entity);
            _context.SaveChanges();
        }

        public bool Exists(int id)
        {
            return _context.UserProjects.Any(t => t.Id == id);
        }
    }
}
