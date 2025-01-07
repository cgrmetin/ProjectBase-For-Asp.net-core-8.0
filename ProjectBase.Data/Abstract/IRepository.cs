using ProjectBase.Entity.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ProjectBase.Data.Abstract
{
    public interface IRepository<T> where T : BaseEntity
    {
        DbSet<T> Table { get; }

        IQueryable<T> GetAll(bool tracking = true);
        IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = true);
        T GetSingle(Expression<Func<T, bool>> method, bool tracking = true);
        T GetById(int id, bool tracking = true);


        bool Add(T entity);
        bool AddRange(List<T> entities);
        bool Remove(T entity);
        bool Update(T entity);
        int Save();

        Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true);
        Task<T> GetByIdAsync(int id, bool tracking = true);
        Task<bool> AddAsync(T entity);
        Task<bool> AddRangeAsync(List<T> entities);
        Task<bool> RemoveAsync(int id);
        Task<int> SaveAsync();
    }
}
