using System.Numerics;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace JobPortalSystem.Models
{
    public class JobPortalApp
    {
        private List<Job> jobs = new List<Job>();
    // Paths to the XML files
    private string userDataFile = Path.Combine(Directory.GetCurrentDirectory(), "Data", "users.xml");
    private string applicationDataFile = Path.Combine(Directory.GetCurrentDirectory(), "Data", "applications.xml");
    private string jobsDataFile = Path.Combine(Directory.GetCurrentDirectory(), "Data", "jobs.xml");

        
    // Registers user
    public void Register()
    {
        Console.WriteLine("Please enter Email : ");
        string email = Console.ReadLine();

        Console.WriteLine("Please enter Password : ");
        string password = Console.ReadLine();

        Console.WriteLine("Please re-enter Password : ");
        string rePassword = Console.ReadLine();

        Console.WriteLine("Please choose role, 1. Job Seeker, 2. Employer ");
        string role = Console.ReadLine();

        // Checking role input
        if (role == "1")
            role = "Seeker";
        else if (role == "2")
            role = "Employer";
        else
        {
            Console.WriteLine("Please Choose 1 or 2.");
            return;
        }

        // Checking if passwords match
        if (password == rePassword)
        {
            // Check if the user already exists by loading from XML
            var existingUsers = LoadUsersFromXml();
            if (existingUsers.Any(u => u.Email == email))
            {
                Console.WriteLine($"Email '{email}' is already taken, try another email.");
                return;
            }

            // Hash password and store the salt
            byte[] salt = GenerateSalt();
            string passwordHash = HashPassword(password, salt);

            // Create a new user object
            var newUser = new User
            {
                UserId = existingUsers.Count + 1,
                Email = email,
                PasswordHash = passwordHash,
                Salt = Convert.ToBase64String(salt),
                Role = role,
                BanStatus = false 
            };

            // Save the new user to XML
            SaveUserToXml(newUser);
            Console.WriteLine("Account created successfully!");
        }
        else
        {
            Console.WriteLine("Passwords do not match.");
        }
    }

    // Generates Salt
    private byte[] GenerateSalt(int size = 16)
    {
        byte[] salt = new byte[size];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    // Hashing password
    private string HashPassword(string password, byte[] salt)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000)) // 10000 iterations
        {
            byte[] hash = pbkdf2.GetBytes(32); // 32-byte hash
            return Convert.ToBase64String(hash); // Store as Base64 string
        }
    }

    // Save user to XML
    private void SaveUserToXml(User user)
    {
        // Load existing users
        XElement usersElement = File.Exists(userDataFile) ? XElement.Load(userDataFile) : new XElement("Users");

        // Create a new XML element for the user
        XElement newUserElement = new XElement("User",
            new XElement("UserId", user.UserId),
            new XElement("Email", user.Email),
            new XElement("PasswordHash", user.PasswordHash),
            new XElement("Salt", user.Salt),
            new XElement("Role", user.Role)
        );

        // Add the new user to the users list in the XML
        usersElement.Add(newUserElement);

        // Save the XML file
        usersElement.Save(userDataFile);
    }
 
    private void SaveApplicationToXml(Application application)
    {
        XElement applicationElement = File.Exists(applicationDataFile) ? XElement.Load(applicationDataFile) : new XElement("Applications");

        XElement newApplicationElement = new XElement("Application",
            new XElement("ApplicationId", application.ApplicationId),
            new XElement("JobId", application.JobId),
            new XElement("UserId", application.UserId),
            new XElement("CoverLetter", application.CoverLetter),
            new XElement("ApplicationDate", application.ApplicationDate)
        );

        applicationElement.Add(newApplicationElement);
        applicationElement.Save(applicationDataFile);

    }
    private void SaveJobToXml(Job job)
    {
        XElement jobsElement = File.Exists(jobsDataFile) ? XElement.Load(jobsDataFile) : new XElement("Jobs");

        XElement newJobsElement = new XElement("Job",
            new XElement("JobId", job.JobId),
            new XElement("JobTitle", job.JobTitle),
            new XElement("CompanyName", job.CompanyName),
            new XElement("Location", job.Location),
            new XElement("JobDescription", job.JobDescription),
            new XElement("Salary", job.Salary)
        );

        jobsElement.Add(newJobsElement);
        jobsElement.Save(jobsDataFile);
        System.Console.WriteLine("saved");
    }
    // Load users from XML
    private List<User> LoadUsersFromXml()
    {
        if (!File.Exists(userDataFile)) return new List<User>();

        XElement usersElement = XElement.Load(userDataFile);

        var users = from userElement in usersElement.Elements("User")
                    select new User
                    {
                        UserId = (int)userElement.Element("UserId"),
                        Email = (string)userElement.Element("Email"),
                        PasswordHash = (string)userElement.Element("PasswordHash"),
                        Salt = (string)userElement.Element("Salt"),
                        Role = (string)userElement.Element("Role"),
                        BanStatus = (bool)userElement.Element("BanStatus")
                    };

        return users.ToList();
    }
    private List<Job> LoadJobFromXml()
    {
        if (!File.Exists(jobsDataFile)) return new List<Job>();

        XElement jobsElement = XElement.Load(jobsDataFile);

        var jobs = from jobElement in jobsElement.Elements("Job")
                    select new Job
                    {
                        JobId = (int)jobElement.Element("JobId"),
                        JobTitle = (string)jobElement.Element("JobTitle"),
                        CompanyName = (string)jobElement.Element("CompanyName"),
                        Location = (string)jobElement.Element("Location"),
                        JobDescription = (string)jobElement.Element("JobDescription"),
                        Salary = (string)jobElement.Element("Salary")
                    };

        return jobs.ToList();
    }

    private List<Application> LoadApplicationsFromXml()
    {
        if(!File.Exists(applicationDataFile)) return new List<Application>();

        XElement applicationElement = XElement.Load(applicationDataFile);

        var applications = applicationElement
        .Elements("Application")  
        .Select(appElement => new Application
                                    (
                                    (int)appElement.Element("ApplicationId"),
                                    (int)appElement.Element("JobId"),
                                    (int)appElement.Element("UserId"),
                                    (string)appElement.Element("CoverLetter"),
                                    (DateTime)appElement.Element("ApplicationDate")
                                    ));
        return applications.ToList();
    }
    // User login
    public User Login()
    {
        Console.WriteLine("Please enter Email : ");
        string email = Console.ReadLine();

        Console.WriteLine("Please enter Password : ");
        string password = Console.ReadLine();

        var users = LoadUsersFromXml();

        User authorized = users.FirstOrDefault(u => u.Email == email);
        if (authorized != null && VerifyPassword(password, authorized.PasswordHash, Convert.FromBase64String(authorized.Salt)))
        {
            Console.WriteLine($"You logged in '{authorized.Email}' successfully, as {authorized.Role} role!");
            return authorized;
        }
        else
        {
            Console.WriteLine("Account does not exist or password is incorrect, please register.");
            return null;
        }
    }

    // Verify password against the stored hash
    private bool VerifyPassword(string enteredPassword, string storedHash, byte[] storedSalt)
    {
        string enteredHash = HashPassword(enteredPassword, storedSalt);
        return storedHash == enteredHash;
    }

    public void CreateNewJob()
    {
        var jobs = LoadJobFromXml();

        var jobId = jobs.Count + 1;

        System.Console.WriteLine("Enter Job Title : ");
        var jobTitle = Console.ReadLine();

        System.Console.WriteLine("Enter Company Name : ");
        var CompanyName = Console.ReadLine();

        System.Console.WriteLine("Enter Location : ");
        var Location = Console.ReadLine();

        System.Console.WriteLine("Enter Job Description : ");
        var JobDescription = Console.ReadLine();

        System.Console.WriteLine("Enter Salary : ");
        var Salary = Console.ReadLine();

        var newJob = new Job
        {
            JobId = jobId,
            JobTitle = jobTitle,
            CompanyName = CompanyName,
            Location = Location,
            JobDescription = JobDescription,
            Salary = Salary,
        };

        SaveJobToXml(newJob);
        System.Console.WriteLine("Job created successfully!");

    }
    public void DisplayJobs()
    {
        List<Job> jobs = LoadJobFromXml();
        jobs.ForEach(j => j.DisplayJobDetails());
    }
    public void ApplyForJob(int userId, int jobId, string coverLetter)
    {
        var applications = LoadApplicationsFromXml();
        int newApplicationId = applications.Count + 1; // Simple incrementing logic for ApplicationId
        DateTime applicationDate = DateTime.Now;
        var jobs = LoadJobFromXml();
        var jobExists = jobs.FirstOrDefault(j => j.JobId == jobId);
        if(jobExists != null)
        {
            Application newApplication = new Application(newApplicationId, jobId, userId, coverLetter, applicationDate);
            applications.Add(newApplication);
            SaveApplicationToXml(newApplication);
            Console.WriteLine("Your application has been successfully submitted.");
        }
        else {
            System.Console.WriteLine("Job Id is incorrect!");
        }
        
    }

    // TODO : 
    public void DisplayJobApplications()
    {
        var users = LoadUsersFromXml();
        var jobs = LoadJobFromXml();
        var applications = LoadApplicationsFromXml();
        if (applications.Any())
        {
            Console.WriteLine($"All the Applications");
            
            foreach (var application in applications)
            {
                var user = users.FirstOrDefault(u => u.UserId == application.UserId); // Find the user by UserId
                if (user != null)
                {
                    Console.WriteLine($"Job Id: {application.JobId} - User: {user.Email} - Cover Letter: {application.CoverLetter} - Applied on: {application.ApplicationDate}");
                }
            }
        }
        else
        {
            Console.WriteLine("No applications for this job.");
        }
    }

    public void DisplayUsers()
    {
        var users = LoadUsersFromXml();
        if(users.Any())
        {
            foreach (var user in users)
            {
                user.DisplayInfo();
            }
        }
    }
    public void ShowMainMenu()
    {
        Console.WriteLine("1. Register");
        Console.WriteLine("2. Login");
        Console.WriteLine("3. Exit");
    }

    public void ShowJobSeekerMenu()
    {
        Console.WriteLine("1. View Job Listings");
        Console.WriteLine("2. Apply for Job");
        Console.WriteLine("3. Logout");
    }

    public void ShowEmployerMenu()
    {
        Console.WriteLine("1. Post New Job");
        Console.WriteLine("2. View Applications");
        Console.WriteLine("3. Logout");
    }

    // For Admin Users
    public void DeleteUser(int UserId)
    {
        XElement userData = XElement.Load(userDataFile);

        var userToDelete = FindUser(UserId, userData);

        if(userToDelete != null)
        {
            userToDelete.Remove();
            userData.Save(userDataFile);
            Console.WriteLine($"User with ID {UserId} has been deleted.");
        }else 
        {
            Console.WriteLine($"User with ID {UserId} not found.");
        }
        
    }
    public void BanUser(int UserId)
    {
        XElement userData = XElement.Load(userDataFile);

        var userToBan = FindUser(UserId, userData);

        if(userToBan != null)
        {
            userToBan.Element("BanStatus").Value = true.ToString();
            userData.Save(userDataFile);
            System.Console.WriteLine($"User with ID {UserId} has been banned.");
        }else 
        {
            System.Console.WriteLine($"User with ID {UserId} not found");
        }
    }
    public void ChangeUserRole(int UserId, string Role)
    {
        XElement userData = XElement.Load(userDataFile);

        var userToChangeRole = FindUser(UserId, userData);

        if(userToChangeRole != null)
        {   
            userToChangeRole.Element("Role").Value = Role;
            userData.Save(userDataFile);
            System.Console.WriteLine($"User with ID {UserId}'s Role has changed to {Role}.");
        }else 
        {
            System.Console.WriteLine($"User with ID {UserId} not found");
        }
    }

    public XElement FindUser(int UserId, XElement userData)
    {
        var foundUser = userData.Elements("User").FirstOrDefault(u => (int)u.Element("UserId") == UserId);

        return foundUser;
    }
    // TODO : create admin user
    public void ShowAdminMenu()
    {
        Console.WriteLine("1. View All Users");
        Console.WriteLine("2. View All Jobs");
        Console.WriteLine("3. Logout");
    }

    public void AdminUserOptions()
    {
        Console.WriteLine("1. Delete User");
        Console.WriteLine("2. Ban User");
        Console.WriteLine("3. Change Role To User");
        Console.WriteLine("4. Back To Menu");
    }

    }
}

