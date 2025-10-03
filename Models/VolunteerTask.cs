using System;
using System.ComponentModel.DataAnnotations;

namespace DisasterManagement.Models
{
    public class VolunteerTask
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Link to ASP.NET Identity User
        public string? AssignedVolunteerId { get; set; }
        public ApplicationUser? AssignedVolunteer { get; set; }

        // Link to VolunteerProfile
        public int? VolunteerProfileId { get; set; }
        public VolunteerProfile? VolunteerProfile { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Open";
    }
}
