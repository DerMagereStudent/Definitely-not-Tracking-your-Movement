using DNTYD.Infrastructure.Database.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DNTYD.Infrastructure.Database; 

public class ApplicationDbContext : IdentityDbContext {
	public DbSet<TrackingPointModel> TrackingPoints { get; set; }

	public ApplicationDbContext(DbContextOptions options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder builder) {
		base.OnModelCreating(builder);

		builder.Entity<TrackingPointModel>()
			.HasOne<IdentityUser>(p => p.User)
			.WithMany().HasForeignKey(p => p.UserId).IsRequired();
			
		builder.Entity<TrackingPointModel>().HasKey(p => new {p.UserId, p.TimeStampTracked});
	}
}