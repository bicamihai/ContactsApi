using ContactsApi.Models.Enums;

namespace ContactsApi.Models
{
    public class ContactSkill
    {
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
        public SkillEnum SkillLevel { get; set; }
    }
}
