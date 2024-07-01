namespace PurpleIK.UI.Areas.CompanyManager.Models.VM.ShiftVM
{
    public class ShiftIndexVM
    {
        public Guid Id { get; set; }
        public string? Employee { get; set; }
        public string Picture { get; set; }
        public string? Name { get; set; }
        public DateTime? ShiftDate { get; set; }
        public TimeSpan? TotalShiftTime { get; set; }     
        public TimeSpan? TotalBreakTimeOne { get; set; }
        public TimeSpan? TotalBreakTimeSecond { get; set; }

    }
}
