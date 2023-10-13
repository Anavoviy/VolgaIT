using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using VolgaIT.Model.Entities;
using VolgaIT.Model.Interfaces;

namespace VolgaIT.EntityDB.Interfaces
{
    public interface IBaseRepository
    {
        public Task SaveChangesAsync();

        public T GetById<T>(long Id) where T : class, IEntity;
        public IQueryable<T> GetBySelector<T>(Expression<Func<T, bool>> selector) where T : class, IEntity;
        public IQueryable<T> GetAll<T>() where T : class, IEntity;

        public Task Add<T>(T newEntity) where T : class, IEntity;
        public Task AddRange<T>(IEnumerable<T> newEntities) where T : class, IEntity;

        public Task Delete<T>(T entity) where T : class, IEntity;
        public Task DeleteRange<T>(IEnumerable<T> newEntities)where T : class, IEntity;

        public Task Remove<T>(T entity) where T : class, IEntity;
        public Task RemoveRange<T>(IEnumerable<T> newEntities) where T : class, IEntity;

        public Task Update<T>(T updateEntity) where T : class, IEntity;
        public Task UpdateRange<T>(IEnumerable<T> updateEntities) where T : class, IEntity;
    }
}
