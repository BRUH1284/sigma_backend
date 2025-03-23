using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_backend.DataTransferObjects
{
    public class ActivityDto
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public int KcalPerHour { get; set; }
    }
}