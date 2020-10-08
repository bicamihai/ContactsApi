using System.Collections.Generic;
using System.Threading.Tasks;
using ContactsApi.Data.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ContactsApi.Data
{
    public interface IContactContext
    {
        Task<List<Skill>> GetSkillsAsync();
        Task<List<SkillLevel>> GetSkillLevelsAsync();
        Task<Skill> GetSkillAsync(int id);
        Skill GetSkill(int id);
        Task<EntityEntry<Skill>> AddSkillAsync(Skill skill);
        Task<int> SaveChangesAsync();
        Task<ContactSkill> GetContactSkillAsync(int skillId, int contactId);
        Task<SkillLevel> GetSkillLevelAsync(int skillLevelCode);
        Task<Contact> GetContactAsync(int contactId);
        Task<EntityEntry<ContactSkill>> AddContactSkillsAsync(ContactSkill contactSkill);
        void Remove<T>(T entity);
        Task<List<Contact>> GetContactsForUserAsync(string userId);
        Task<List<ContactSkill>> GetContactSkills(int contactId, string userId);
        Task<EntityEntry<Contact>> AddContactAsync(Contact contact);
        Contact GetContact(int id);
    }
}
