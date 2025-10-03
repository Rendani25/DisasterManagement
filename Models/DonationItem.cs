using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace DisasterManagement.Models
{
    public class DonationItem
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int DonationId { get; set; }

        [ForeignKey("DonationId")]
        public Donation Donation { get; set; }


    }
}



