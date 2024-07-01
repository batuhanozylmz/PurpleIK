using PurpleIK.Entities;

namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.ShiftVM
{
    public class ShiftEditVM
    {
        public ShiftEditItem ShiftEditItem { get; set; }
        public List<Person>? Persons { get; set; }
    }
    public class ShiftEditItem
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime? ShiftDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public TimeSpan? BreakTimeOneStart { get; set; }
        public TimeSpan? BreakTimeOneEnd { get; set; }
        public TimeSpan? BreakTimeSecondStart { get; set; }
        public TimeSpan? BreakTimeSecondEnd { get; set; }
    }
}
