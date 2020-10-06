using System.Collections.Generic;
using ContactsApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactsApi.Data
{
    public class ContactContext : DbContext
    {
        public ContactContext(DbContextOptions<ContactContext> options) : base(options) {}
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<ContactSkill> ContactSkills { get; set; }
        public DbSet<SkillLevel> SkillLevels { get; set; }

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

            modelBuilder.Entity<SkillLevel>()
                        .HasData(new List<SkillLevel>
                        {
                            new SkillLevel
                            {
                                Id = 1,
                                LevelCode = 1,
                                LevelDescription = "Beginner"
                            },
                            new SkillLevel
                            {
                                Id = 2,
                                LevelCode = 2,
                                LevelDescription = "Intermediate"
                            },
                            new SkillLevel
                            {
                                Id = 3,
                                LevelCode = 3,
                                LevelDescription = "Advanced"
                            }
                        });

            modelBuilder.Entity<Skill>()
                        .HasData(new List<Skill>
                        {
                            new Skill
                            {
                                Id = 1,
                                Name = "DrinkingBeer",
                                SkillCode = 1
                            },
                            new Skill
                            {
                                Id = 2,
                                Name = "RidingBike",
                                SkillCode = 2
                            },
                            new Skill
                            {
                                Id = 3,
                                Name = "SingingKaraoke",
                                SkillCode = 3
                            }
                        });
        }
    }
}
