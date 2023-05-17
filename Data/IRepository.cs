namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public interface IRepository<T> where T : class
    {
        //Create
        void Create(T entity);

        //Read
        T? Get(int? Id);
        ICollection<T> GetAll();

        //Update
        void Update(T entity);

        //Delete
        void Delete(T entity);
    }

}

