using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class ProjectsRepository : IRepository<Project>
    {
        //DbContext
        private ApplicationDbContext _context;

        public ProjectsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //CRUD methods
        public void Create(Project entity)
        {
            _context.Projects.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(Project entity)
        {
            _context.Projects.Remove(entity);
            _context.SaveChanges();
        }

        public Project? Get(int? Id)
        {
            return _context.Projects.Find(Id);
        }

        public ICollection<Project> GetAll()
        {
            return _context.Projects.ToList<Project>();
        }

        public void Update(Project entity)
        {
            _context.Projects.Update(entity);  
        }
    }
}
