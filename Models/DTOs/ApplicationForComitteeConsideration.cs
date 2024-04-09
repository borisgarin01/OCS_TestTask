namespace OCS_TestTask.Models.DTOs
{
    public sealed class ApplicationForComitteeConsideration
    {
        public Guid Id { get; init; }
        public Guid Application_Id { get; init; }
        public DateTime SubmittingTimeStamp { get; init; } = DateTime.UtcNow;
    }
}
