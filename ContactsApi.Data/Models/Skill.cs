using System.Collections.Generic;

namespace ContactsApi.Data.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public int SkillCode { get; set; }
        public string Name{ get; set; }
        public virtual ICollection<ContactSkill> ContactSkills { get; set; }
    }
}
