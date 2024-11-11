using System.Security.Cryptography.X509Certificates;
using JobPortalSystem;
using JobPortalSystem.Models;

JobPortalApp jobPortal = new JobPortalApp();
bool run = true;
var auth = false;
while(run)
{
    jobPortal.ShowMainMenu();
    var opt = Console.ReadLine();

    switch (opt)
    {
        case "1":
            jobPortal.Register();
            break;
        case "2":
            var authenticatedUser = jobPortal.Login();
            if(authenticatedUser != null)
            {
                auth = true;
                while(auth)
                {
                    if(authenticatedUser.Role == "Seeker")
                        JobSeekerOptions(authenticatedUser);

                    else if(authenticatedUser.Role == "Employer")
                        EmployerOptions(authenticatedUser);
                }
            }
            break;
        case "3":
            run = false;    
            break;
            
    }

    
}
void JobSeekerOptions(User authenticatedUser)
{
    jobPortal.ShowJobSeekerMenu();
    var opt = Console.ReadLine();

    switch (opt)
    {
        case "1":
            jobPortal.DisplayJobs();
            break;
        case "2":
            System.Console.WriteLine("Please enter job ID you want to apply");
            var jobId = int.Parse(Console.ReadLine());
            System.Console.WriteLine("Please enter Cover Letter for a job");
            var coverLetter = Console.ReadLine();

            jobPortal.ApplyForJob(authenticatedUser.UserId, jobId, coverLetter);
            break;
        case "3":
            auth = false;
            System.Console.WriteLine("You Logged Out!");
            break;
    }
}
void EmployerOptions(User authenticatedUser)
{
    jobPortal.ShowEmployerMenu();
    var opt = Console.ReadLine();

    switch (opt)
    {
        case "1":
            jobPortal.CreateNewJob();
            break;
        case "2":
            jobPortal.DisplayJobApplications();
            break;
        case "3":
            auth = false;
            break;
    }

}