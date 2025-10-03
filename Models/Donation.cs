using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DisasterManagement.Models
{
    public class Donation
    {
        [Key]
        public int DonationId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";


        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }

        [Required]
        public string DonorId { get; set; }

        [ForeignKey("DonorId")]
        public ApplicationUser Donor { get; set; } // navigation property to the user

        public ICollection<DonationItem> Items { get; set; }= new List<DonationItem>();
    }
}