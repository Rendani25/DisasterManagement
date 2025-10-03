using System.Collections.Generic;

namespace DisasterManagement.Models
{
    public class HomeViewModel
    {
        public int TotalIncidents { get; set; }
        public int TotalDonations { get; set; }
        public int TotalVolunteers { get; set; }

        public List<IncidentReport>? RecentIncidents { get; set; }
        public List<Donation>? RecentDonations { get; set; }
        public List<VolunteerTask>? RecentTasks { get; set; }
    }
}
