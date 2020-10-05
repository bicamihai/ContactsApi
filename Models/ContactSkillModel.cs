namespace ContactsApi.Models
{
    public class ContactSkillModel
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public SkillLevelModel SkillLevel { get; set; }
    }
}
