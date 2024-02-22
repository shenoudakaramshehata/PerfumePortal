namespace CRM.Repository.GenericRepo
{
    public interface IGenericRepo<T> where T : class
    {
        T? GetById(int id);

        List<T> GetAll();

        void Add(T entity);

        void Delete(T entity);

        void Update(T entity);

        void SaveChanges();
    }
}
