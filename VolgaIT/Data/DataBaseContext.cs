using Microsoft.EntityFrameworkCore;
using VolgaIT.Model.Entities;

namespace VolgaIT.EntityDB
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
            Database.EnsureCreated();

            if (Users == null ||  Users.FirstOrDefault(u => u.Username == " admin") == null)
            {
                Users.Add(new UserEntity() { IsAdmin = true, Balance = 0, Username = "admin", Password = "admin" });
                SaveChanges();
            }
        }

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
