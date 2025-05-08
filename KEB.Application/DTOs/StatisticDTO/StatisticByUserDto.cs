using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.StatisticDTO
{
    public class StatisticByUserDto
    {
        //thong ke theo nguoi dung va thoi gian
        public Guid? UserId { get; set; }
        public DateTime? LowerBound { get; set; }
        public DateTime? UpperBound { get; set; }
    }
}
