using Microsoft.EntityFrameworkCore;

namespace ContactsApi.Models
{
    public class ContactContext : DbContext
    {
        public ContactContext(DbContextOptions<ContactContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactSkill>()
                        .HasKey(bc => new { bc.ContactId, bc.SkillId });
            modelBuilder.Entity<ContactSkill>()
                        .HasOne(bc => bc.Contact)
                        .WithMany(b => b.ContactSkills)
                        .HasForeignKey(bc => bc.ContactId);
            modelBuilder.Entity<ContactSkill>()
                        .HasOne(bc => bc.Skill)
                        .WithMany(c => c.ContactSkills)
                        .HasForeignKey(bc => bc.SkillId);
        }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<ContactSkill> ContactSkills { get; set; }

    }
}
