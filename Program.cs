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
                    if(authenticatedUser.BanStatus == false)
                    {
                        if(authenticatedUser.Role == "Seeker")
                            JobSeekerOptions(authenticatedUser);

                        else if(authenticatedUser.Role == "Employer")
                            EmployerOptions();
                        else if(authenticatedUser.Role == "Admin")
                            AdminOptions();
                    }else
                    {
                        System.Console.WriteLine("You Have Been Banned.");
                        break;
                    }
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
void EmployerOptions()
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
void AdminOptions()
{   
    jobPortal.ShowAdminMenu();
    var opt = Console.ReadLine();

    switch (opt)
    {
        case "1":
            jobPortal.DisplayUsers();
            System.Console.WriteLine("Choose User with ID :");
            int userId = int.Parse(Console.ReadLine());
            jobPortal.AdminUserOptions(); 
            var cmd = Console.ReadLine();

            if(cmd == "1")
                jobPortal.DeleteUser(userId);
            else if(cmd == "2")
                jobPortal.BanUser(userId);
            else if(cmd == "3")
            {
                System.Console.WriteLine("Role : 1. Job Seeker, 2. Employer, 3. Admin");
                var roleCode = Console.ReadLine();
                string role = (roleCode == "1") ? "Seeker" : (roleCode == "2") ? "Employer" : (roleCode == "3") ? "Admin" : "undefined";
                jobPortal.ChangeUserRole(userId, role);
            }
            else if(cmd == "4")
                break;
            break;
            
        case "2":
            jobPortal.DisplayJobs();
            break;
        case "3":
            break;
    }
}