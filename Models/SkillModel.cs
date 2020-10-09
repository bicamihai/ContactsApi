using System.ComponentModel.DataAnnotations;

namespace ContactsApi.Models
{
    public class SkillModel
    {
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
