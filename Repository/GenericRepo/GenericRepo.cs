using CRM.Data;

namespace CRM.Repository.GenericRepo
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public GenericRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public T? GetById(int id)
        {
           return _context.Set<T>().Find(id);
        }

        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
        }
    }
}
