namespace OCS_TestTask.Models
{
    public class ApplicationForComitteeConsideration
    {
        public Guid Id { get; set; }
        public Guid Application_Id { get; set; }
        public DateTime SubmittingTimeStamp { get; set; } = DateTime.UtcNow;
    }
}
