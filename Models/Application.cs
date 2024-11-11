namespace JobPortalSystem.Models
{
    public class Application
    {
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public int UserId { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
        public DateTime ApplicationDate { get; set; }


        public Application(int applicationId, int jobId, int userId, string coverLetter, DateTime applicationDate)
        {
            ApplicationId = applicationId;
            JobId = jobId;
            UserId = userId;
            CoverLetter = coverLetter;
            ApplicationDate = applicationDate;
        }
    }

}