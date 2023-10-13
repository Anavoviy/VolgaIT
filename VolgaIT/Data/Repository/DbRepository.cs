using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using VolgaIT.EntityDB;
using VolgaIT.EntityDB.Interfaces;
using VolgaIT.Model.Interfaces;

namespace VolgaIT.Data.Repository
{
    public class DbRepository : IBaseRepository
    {
        private DataContext _context;

        public DbRepository(DataContext context)
        {
            _context = context;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        async Task IBaseRepository.Add<T>(T newEntity)
        {
            if(newEntity != null)
            {
                await _context.Set<T>().AddAsync(newEntity);
            }
        }

        async Task IBaseRepository.AddRange<T>(IEnumerable<T> newEntities)
        {
            if(newEntities != null && newEntities.Count() > 0)
            {
                await _context.Set<T>().AddRangeAsync(newEntities);
            }
        }

        async Task IBaseRepository.Delete<T>(T entity)
        {
            if(entity != null)
            {
                entity.IsDeleted = true;
                _context.Set<T>().Update(entity);
            }
        }

        async Task IBaseRepository.DeleteRange<T>(IEnumerable<T> newEntities)
        {
            if(newEntities != null && newEntities.Count() > 0)
            {
                foreach(T entity in newEntities)
                    entity.IsDeleted = true;

                _context.Set<T>().UpdateRange(newEntities);
            }
        }
        
        IQueryable<T> IBaseRepository.GetAll<T>()
        {
            return _context.Set<T>().AsQueryable();
        }

        T IBaseRepository.GetById<T>(long Id)
        {
            return _context.Set<T>().FirstOrDefault(t => t.Id ==  Id);
        }

        IQueryable<T> IBaseRepository.GetBySelector<T>(Expression<Func<T, bool>> selector)
        {
            return _context.Set<T>().Where(selector).Where(t => t.IsDeleted == false).AsQueryable();
        }

        Task IBaseRepository.Remove<T>(T entity)
        {
            if(entity != null && entity.Id > 0)
            {
                _context.Set<T>().Remove(entity);
                return Task.CompletedTask;
            }
            return null;
        }

        Task IBaseRepository.RemoveRange<T>(IEnumerable<T> newEntities)
        {
            if (newEntities != null && newEntities.Count() > 0)
            {
                _context.Set<T>().RemoveRange(newEntities);
                return Task.CompletedTask;
            }
            return null;
        }

        Task IBaseRepository.Update<T>(T updateEntity)
        {
            if (updateEntity != null && updateEntity.Id > 0)
            {
                _context.Set<T>().Update(updateEntity);
                return Task.CompletedTask;
            }
            return null;
        }

        Task IBaseRepository.UpdateRange<T>(IEnumerable<T> updateEntities)
        {
            if (updateEntities != null && updateEntities.Count() > 0)
            {
                _context.Set<T>().UpdateRange(updateEntities);
                return Task.CompletedTask;
            }
            return null; ;
        }
    }
}
