using CarsShop.Models;

namespace CarsShop.Interfaces
{
    public interface ICarService
    {
        Task<List<Car>> GetCarsAsync();
        Task AddCarAsync(Car car);
        Task UpdateCarAsync(Car car);
        Task DeleteCarAsync(Car car);
    }
}
