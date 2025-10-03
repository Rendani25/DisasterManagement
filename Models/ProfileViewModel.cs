using System.ComponentModel.DataAnnotations;

namespace DisasterManagement.Models
{
    public class ProfileViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        public bool IsVolunteer { get; set; } = false;
        public bool IsDonor { get; set; } = false;
    }
}
