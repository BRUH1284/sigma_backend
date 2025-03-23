using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_backend.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public int KcalPerHour { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now.ToUniversalTime();
    }
}