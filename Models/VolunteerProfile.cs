using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DisasterManagement.Models
{
    public class VolunteerProfile
    {
        public int Id { get; set; }

        // Link to the ASP.NET Identity user
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [MaxLength(200)]
        public string? Skills { get; set; }

        [MaxLength(100)]
        public string? Availability { get; set; }

        public int HoursContributed { get; set; } = 0;

        // Tasks assigned to the volunteer
        public ICollection<VolunteerTask> Tasks { get; set; } = new List<VolunteerTask>();
    }
}
