using AutoMapper;
using ContactsApi.Data.Models;
using ContactsApi.Models;

namespace ContactsApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ContactModel, Contact>();
            CreateMap<ContactSkillModel, ContactSkill>();
            CreateMap<SkillLevelModel, SkillLevel>();
            CreateMap<SkillModel, Skill>();

            CreateMap<Contact, ContactModel>();
            CreateMap<ContactSkill, ContactSkillModel>();
            CreateMap<SkillLevel, SkillLevelModel>();
            CreateMap<Skill, SkillModel>();
        }
    }
}
