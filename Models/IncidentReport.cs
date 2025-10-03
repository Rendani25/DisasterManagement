using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DisasterManagement.Models
{
    public enum DisasterType
    {
        Flood = 0,
        Earthquake = 1,
        Fire = 2,
        Storm = 3,
        Other = 99
    }

    public class IncidentReport
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; }

        [Required, DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public DisasterType Type { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [Required]
        public string ReportedById { get; set; }
        public ApplicationUser ReportedBy { get; set; }

        public DateTime ReportedOn { get; set; } = DateTime.UtcNow;

        [MaxLength(300)]
        public string? AttachmentPath { get; set; }
        
        [NotMapped]
        public IFormFile? AttachmentFile { get; set; }  // file uploaded from form

        [MaxLength(50)]
        public string Status { get; set; } = "Open";
    }
}
