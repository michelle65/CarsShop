using CarsShop.Models;

namespace CarsShop.Interfaces
{
    public interface IUserService
    {
        Task AddUserAsync(User user);
        Task DeleteUserAsync(User selectedUser);
        Task<List<User>> GetUsersAsync();
        Task UpdateUserAsync(User selectedUser);

        public interface IUserService
        {
            Task AddUserAsync(User user);
            Task UpdateUserAsync(User user);
            Task DeleteUserAsync(User user);
        }
    }
}