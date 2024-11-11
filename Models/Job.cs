namespace JobPortalSystem.Models
{
    public class Job
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string Salary { get; set; }

        public void DisplayJobDetails()
        {
            Console.WriteLine($"Job ID: {JobId}");
            Console.WriteLine($"Title: {JobTitle}");
            Console.WriteLine($"Company: {CompanyName}");
            Console.WriteLine($"Location: {Location}");
            Console.WriteLine($"Salary: ${Salary}");
            Console.WriteLine($"Description: {JobDescription}");
            Console.WriteLine();
        }
    }
}