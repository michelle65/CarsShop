using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsShop.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public virtual ICollection<Car> Cars { get; set; }
    }
}
