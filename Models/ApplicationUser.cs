﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
public class ApplicationUser : IdentityUser
{

    public string? FullName { get; set; }

    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsVolunteer { get; set; }
    public bool IsDonor { get; set; }
}
