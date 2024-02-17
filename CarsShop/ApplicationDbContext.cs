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
            return $"Data Source=desktop\\SQLServer;Initial Catalog=CarsShop;Trusted_Connection=True;";
        }

        public async Task<int> CountCarsAsync(int selectedUserId)
        {
            int count = 0;

            using (var connection = new SqlConnection(ConnectionStringBuild()))
            {
                await connection.OpenAsync();
                string query = "SELECT COUNT(*) FROM Cars WHERE UserId = @UserId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", selectedUserId);
                    count = (int)await command.ExecuteScalarAsync();
                }
                connection.Close();
            }

            return count;
        }

        public async Task<int> CountUsersAsync()
        {
            int count = 0;
            using (var connection = new SqlConnection(ConnectionStringBuild()))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM Users", connection))
                {
                    count = (int)await command.ExecuteScalarAsync();
                }
                connection.Close();
            }
            return count;
        }
    }
}
