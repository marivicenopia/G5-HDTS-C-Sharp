using Microsoft.EntityFrameworkCore;
using ASI.Basecode.WebApp.Models.NexDesk;

namespace ASI.Basecode.WebApp.Data
{
    public class NexDeskDbContext : DbContext
    {
        public NexDeskDbContext(DbContextOptions<NexDeskDbContext> options) : base(options)
        {
        }

        // Map to exact table names
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<TicketAttachment> TicketAttachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure table mappings to match existing database
            modelBuilder.Entity<User>().ToTable("dbUser");
            modelBuilder.Entity<Ticket>().ToTable("ticketsdb");
            modelBuilder.Entity<Article>().ToTable("articlesdb");
            modelBuilder.Entity<Feedback>().ToTable("feedbackdb");
            modelBuilder.Entity<TicketAttachment>().ToTable("ticket_attachments");

            // Configure primary keys
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<Ticket>().HasKey(t => t.Id);
            modelBuilder.Entity<Article>().HasKey(a => a.Id);
            modelBuilder.Entity<Feedback>().HasKey(f => f.Id);
            modelBuilder.Entity<TicketAttachment>().HasKey(ta => ta.Id);

            // Configure indexes for performance
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.SubmittedBy);

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.AssignedTo);

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.Status);

            modelBuilder.Entity<TicketAttachment>()
                .HasIndex(ta => ta.TicketId);

            modelBuilder.Entity<Feedback>()
                .HasIndex(f => f.TicketId);

            // Configure default values
            modelBuilder.Entity<User>()
                .Property(u => u.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedTime)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<User>()
                .Property(u => u.UpdatedTime)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Ticket>()
                .Property(t => t.SubmittedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<TicketAttachment>()
                .Property(ta => ta.UploadDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Feedback>()
                .Property(f => f.Date)
                .HasDefaultValueSql("GETDATE()");

            base.OnModelCreating(modelBuilder);
        }
    }
}