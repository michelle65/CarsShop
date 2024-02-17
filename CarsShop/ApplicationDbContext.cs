using CarsShop.Models;
using System.Data.Entity;
using System.Data.SqlClient;

namespace CarsShop
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public ApplicationDbContext() : base(ConnectionStringBuild()) { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Cars)
                .WithRequired(p => p.User)
                .HasForeignKey(u => u.UserId);
        }
        private static string ConnectionStringBuild()
        {
            return $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CarsShop;Trusted_Connection=True;";
        }

        public int CountCars()
        {
            int count = 0;
            // Create a new connection for each call.
            using (var connection = new SqlConnection(ConnectionStringBuild()))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM Cars", connection))
                {
                    count = (int)command.ExecuteScalar();
                }
            }
            return count;
        }

        public int CountUsers()
        {
            int count = 0;
            // Create a new connection for each call.
            using (var connection = new SqlConnection(ConnectionStringBuild()))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM Users", connection))
                {
                    count = (int)command.ExecuteScalar();
                }
            }
            return count;
        }
    }
}
