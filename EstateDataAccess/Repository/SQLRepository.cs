using EstateDataAccess.Repository;
using System.Collections.Generic;
using System.Configuration;

public abstract class SQLRepository<T> : IRepository<T>
{
    protected readonly string ConnectionString;

    public SQLRepository()
    {
        ConnectionString = ConfigurationManager.ConnectionStrings["EstateManagementDb"].ConnectionString;
    }

    public abstract T GetById(int id);
    public abstract List<T> GetAll();
    public abstract T Create(T entity);
    public abstract T Update(T entity);
    public abstract void Delete(int id);

    public abstract IEnumerable<T> SearchAndSort(string searchTerm, string sortBy, bool descending);
}
