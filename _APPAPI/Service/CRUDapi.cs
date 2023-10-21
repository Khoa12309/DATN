using APPDATA.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace _APPAPI.Service
{
    public class CRUDapi<T> where T : class
    {
        ShoppingDB _context;
        DbSet<T> _dbSet;
      
        public CRUDapi(ShoppingDB context, DbSet<T> dbset)
        {
            _context = context;
            _dbSet = dbset;
        }
        public bool CreateItem(T item)
        {
            try
            {
                _dbSet.Add(item);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public bool DeleteItem(T item)
        {
            try
            {
                _dbSet.Remove(item);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public IEnumerable<T> GetAllItems()
        {
            return _dbSet.ToList();
        }

        public bool UpdateItem(T item)
        {
            try
            {
                _dbSet.Update(item);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}

