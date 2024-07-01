using PurpleIK.Core.Entity.Abstract;
using PurpleIK.Core.Entity.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Entities
{
    public class Shift : BaseEntity<Guid>, IEntity<Guid>
    {
        public string? Name { get; set; }

        public DateTime? ShiftDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public TimeSpan? BreakTimeOneStart { get; set; }
        public TimeSpan? BreakTimeOneEnd { get; set; }
        public TimeSpan? BreakTimeSecondStart { get; set; }
        public TimeSpan? BreakTimeSecondEnd { get; set; }

        public Guid? PersonId { get; set; }

        //nav
        public virtual Person? Person { get; set; }
    }

}
