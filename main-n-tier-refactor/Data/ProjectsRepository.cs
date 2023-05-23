using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models;
using System.Linq.Expressions;

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

        public bool Exists(int id)
        {
            return _context.Projects.Any(t => t.Id == id);
        }

        public Project? Get(int? Id)
        {
            return _context.Projects.Find(Id);
        }

        public ICollection<Project> GetAll()
        {
            return _context.Projects.ToList();
        }

        public void Update(Project entity)
        {
            _context.Projects.Update(entity);
            _context.SaveChanges();
        }

        public IQueryable<Project> Include(params Expression<Func<Project, object>>[] includeExpressions)
        {
            DbSet<Project> dbSet = _context.Set<Project>();

            IQueryable<Project> query = null;
            foreach (var includeExpression in includeExpressions)
            {
                query = dbSet.Include(includeExpression);
            }

            return null ?? dbSet;
        }
    }
}
