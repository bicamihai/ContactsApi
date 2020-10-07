using System.ComponentModel.DataAnnotations;

namespace ContactsApi.Models
{
    public class SkillLevelModel
    {
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int Id { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int LevelCode { get; set; }
        [Required]
        public string LevelDescription { get; set; }
    }
}
