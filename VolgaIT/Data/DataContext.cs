using Microsoft.EntityFrameworkCore;
using VolgaIT.Model.Entities;

namespace VolgaIT.EntityDB
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}

       

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<TransportEntity> Transports { get; set; }
        public DbSet<RentEntity> Rents { get; set; }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public DbSet<T> DbSet<T>() where T : class
        {
            return Set<T>();
        }

        public new IQueryable<T> Query<T>() where T : class
        {
            return Set<T>();
        }


    }
}
