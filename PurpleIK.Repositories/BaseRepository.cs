using PurpleIK.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class,new()
    {
        private readonly PurpleDbContext db;

        public BaseRepository(PurpleDbContext context)
        {
            this.db = context;
        }

        public void Add(T item)
        {
            try
            {
                db.Set<T>().Add(item);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new Exception("Ekleme İşlemi Sırasında Hata Meydana Geldi");
            }
        }

        public void Edit(T item)
        {
            try
            {
                db.Set<T>().Update(item);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new Exception("Güncelleme İşlemi Sırasında Hata Meydana Geldi");
            }
        }

        public T Find(Guid id)
        {
            return db.Set<T>().Find(id);
        }

        public List<T> GetAll()
        {
             return db.Set<T>().ToList();
        }

        public List<T> GetBy(Func<T, bool> exp)
        {
            return db.Set<T>().Where(exp).ToList();
        }

        public T GetByEntity(Func<T, bool> exp)
        {
            return db.Set<T>().Where(exp).FirstOrDefault();
        }

        public List<T> GetDeActive()
        {
            //PropertyInfo kullanılacak
            return null;
        }

        public void Remove(T item)
        {
            try
            {
                db.Set<T>().Remove(item);
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Silme İşlemi Sırasında Hata Meydana Geldi");
            }
        }
        public int CountBy(Func<T, bool> exp)
        {
            return db.Set<T>().Count(exp);
        }
    }
}
