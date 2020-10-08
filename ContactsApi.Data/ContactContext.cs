using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactsApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ContactsApi.Data
{
    public class ContactContext : DbContext, IContactContext
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
                        .HasForeignKey(bc => bc.ContactId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ContactSkill>()
                        .HasOne(bc => bc.Skill)
                        .WithMany(c => c.ContactSkills)
                        .HasForeignKey(bc => bc.SkillId)
                        .OnDelete(DeleteBehavior.Cascade);

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

        public async Task<List<Skill>> GetSkillsAsync()
        {
            return await Skills.ToListAsync();
        }

        public async Task<List<SkillLevel>> GetSkillLevelsAsync()
        {
            return await SkillLevels.ToListAsync();
        }

        public async Task<Skill> GetSkillAsync(int id)
        {
            return await Skills.FindAsync(id);
        }

        public Skill GetSkill(int id)
        {
            return Skills.Find(id);
        }

        public async Task<EntityEntry<Skill>> AddSkillAsync(Skill skill)
        {
            return await Skills.AddAsync(skill);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public async Task<ContactSkill> GetContactSkillAsync(int contactId, int skillId)
        {
            return await ContactSkills.FirstOrDefaultAsync(x => x.SkillId == skillId && x.ContactId == contactId);
        }

        public async Task<SkillLevel> GetSkillLevelAsync(int skillLevelCode)
        {
            return await SkillLevels.FirstOrDefaultAsync(x => x.LevelCode == skillLevelCode);
        }

        public async Task<Contact> GetContactAsync(int contactId)
        {
            return await Contacts.FindAsync(contactId);
        }

        public Contact GetContact(int id)
        {
            return Contacts.Find(id);
        }

        public async Task<EntityEntry<ContactSkill>> AddContactSkillsAsync(ContactSkill contactSkill)
        {
            return await ContactSkills.AddAsync(contactSkill);
        }

        void IContactContext.Remove<T>(T entity)
        {
            Remove(entity);
        }

        public async Task<List<Contact>> GetContactsForUserAsync(string userId)
        {
            return await Contacts.Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task<List<ContactSkill>> GetContactSkillsAsync(int contactId)
        {
            return await ContactSkills.Where(c => c.ContactId == contactId)
                                      .Include(x => x.Skill)
                                      .Include(x => x.SkillLevel)
                                      .ToListAsync();
        }

        public async Task<EntityEntry<Contact>> AddContactAsync(Contact contact)
        {
            return await Contacts.AddAsync(contact);
        }
    }
}
