using System.Collections.Generic;
using ContactsApi.Models.Enums;

namespace ContactsApi.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name{ get; set; }
        public virtual ICollection<ContactSkill> ContactSkills { get; set; }
    }
}
