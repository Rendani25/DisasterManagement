using DisasterManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DisasterManagement.data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<IncidentReport> IncidentReports { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<DonationItem> DonationItems { get; set; }
        public DbSet<VolunteerProfile> VolunteerProfiles { get; set; }
        public DbSet<VolunteerTask> VolunteerTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Donation>()
                .HasMany(d => d.Items)
                .WithOne(i => i.Donation)
                .HasForeignKey(i => i.DonationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<VolunteerTask>()
                .HasOne(t => t.VolunteerProfile)
                .WithMany(v => v.Tasks)
                .HasForeignKey(t => t.VolunteerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VolunteerTask>()
                .HasOne(t => t.AssignedVolunteer)
                .WithMany()
                .HasForeignKey(t => t.AssignedVolunteerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
