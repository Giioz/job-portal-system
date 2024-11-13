
namespace JobPortalSystem.Models
{
    public class User
    {
        public int UserId { get; set;}
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string Salt { get; set; } // Store the salt as a byte array
        public bool BanStatus { get; set; } = false;

        public void DisplayInfo()
        {
            System.Console.WriteLine($"User Id : {UserId} - Email Adress : {Email} - Role : {Role}");
        }
    }
}