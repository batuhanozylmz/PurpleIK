using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Entities
{
    public class PublicHolidays
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime? Date { get; set; }
        public int? NumberOfDays { get; set; }
        public string? Color { get; set; }
    }
}
