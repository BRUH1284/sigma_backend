using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_backend.DataTransferObjects.Activity
{
    public class UpdateActivityRequestDto
    {
        public String Name { get; set; }
        public int KcalPerHour { get; set; }
    }
}