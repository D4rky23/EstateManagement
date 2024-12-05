
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstateDataAccess.Repository
{
    public interface IRepository<T>
    {
        T GetById(int id);
        List<T> GetAll();
        T Create(T entity);
        T Update(T entity);
        void Delete(int id);
        IEnumerable<T> SearchAndSort(string searchTerm, string sortBy, bool descending);

    }
}