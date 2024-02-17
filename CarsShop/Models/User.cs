﻿namespace CarsShop.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public virtual ICollection<Car> Cars { get; set; }
    }
}
