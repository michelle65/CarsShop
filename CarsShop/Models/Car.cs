namespace CarsShop.Models
{
    public class Car
    {
        public int CarId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}