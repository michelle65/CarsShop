using CarsShop.Interfaces;
using CarsShop.Models;
using System.Data.Entity;

namespace CarsShop.Services
{
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext _dbContext;

        public CarService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Car>> GetCarsAsync()
        {
            return await _dbContext.Cars.ToListAsync();
        }

        public async Task AddCarAsync(Car car)
        {
            _dbContext.Cars.Add(car);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCarAsync(Car car)
        {
            _dbContext.Entry(car).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCarAsync(Car car)
        {
            _dbContext.Cars.Remove(car);
            await _dbContext.SaveChangesAsync();
        }
    }
}