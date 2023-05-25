using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class CommentRepository : IRepository<Comment>
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(Comment entity)
        {
            _context.Comments.Add(entity);
            _context.SaveChanges();
        }

        public Comment? Get(int? Id)
        {
            return _context.Comments.FirstOrDefault(c => c.Id == Id);
        }

        public ICollection<Comment> GetAll()
        {
            return _context.Comments.ToList();
        }

        public void Update(Comment entity)
        {
            _context.Comments.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(Comment entity)
        {
            _context.Comments.Remove(entity);
            _context.SaveChanges();
        }

        public bool Exists(int id)
        {
            return _context.Comments.Any(t => t.Id == id);
        }
    }
}
