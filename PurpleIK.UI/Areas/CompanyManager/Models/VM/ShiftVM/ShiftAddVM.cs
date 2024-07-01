using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.ShiftVM
{
        public class ShiftAddVM
        {

            public ShiftAddItem ShiftAddItem { get; set; }
            public List<Person>? Persons { get; set; }
        }
        public class ShiftAddItem
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
        }
    }
