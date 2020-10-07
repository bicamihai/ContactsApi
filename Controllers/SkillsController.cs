using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContactsApi.Data;
using ContactsApi.Data.Models;
using ContactsApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ContactsApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SkillsController : BaseController
    {
        private readonly ContactContext _context;

        public SkillsController(ContactContext context, ApplicationDbContext applicationDbContext, IMapper mapper) : base(applicationDbContext, mapper)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a list of all skills in the database.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SkillModel>>> GetSkills()
        {
            var skills = await _context.Skills.ToListAsync();
            return new ActionResult<IEnumerable<SkillModel>>(Mapper.Map<IEnumerable<SkillModel>>(skills));
        }

        /// <summary>
        /// Gets a specific skill.
        /// </summary>
        /// <param name="id"></param> 
        [HttpGet("{id}")]
        public async Task<ActionResult<SkillModel>> GetSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound(Resources.SkillNotFound);
            }

            return new ActionResult<SkillModel>(Mapper.Map<SkillModel>(skill));
        }

        /// <summary>
        /// Edits the details for a specific skill.
        /// </summary>
        /// <param name="skillModel"></param> 
        [HttpPut]
        public async Task<IActionResult> PutSkill(SkillModel skillModel)
        {
            var skill = await _context.Skills.FindAsync(skillModel.Id);
            if (skill == null)
            {
                return NotFound(Resources.SkillNotFound);
            }
            Mapper.Map(skillModel, skill);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillExists(skillModel.Id))
                {
                    return NotFound(Resources.SkillNotFound);
                }
                throw;
            }
            return Ok(Resources.SkillUpdated);
        }

        /// <summary>
        /// Adds a skill.
        /// </summary>
        /// <param name="skillModel"></param> 
        [HttpPost]
        public async Task<ActionResult<SkillModel>> PostSkill(SkillModel skillModel)
        {
            var skill = Mapper.Map<Skill>(skillModel);
            skill.Id = 0;
            await _context.Skills.AddAsync(skill);
            await _context.SaveChangesAsync();
            skillModel.Id = skill.Id;
            return CreatedAtAction("GetSkill", new { id = skillModel.Id }, skillModel);
        }

        /// <summary>
        /// Adds a skill with a skill level to a customer.
        /// </summary>
        /// <param name="skillId"></param> <param name="contactId"></param> <param name="skillLevelCode"></param>
        [HttpPost("/AddSkillToContact")]
        public async Task<ActionResult> AddSkillToContact(int skillId, int contactId, int skillLevelCode)
        {
            if (_context.ContactSkills.Any(x => x.SkillId == skillId && x.ContactId == contactId))
            {
                return BadRequest(Resources.ContactSkillExists);
            }
            if (!_context.Skills.Any(x => x.Id == skillId))
            {
                return NotFound(Resources.SkillNotFound);
            }
            if (!_context.SkillLevels.Any(x => x.LevelCode == skillLevelCode))
            {
                return NotFound(Resources.SkillLevelNotFound);
            }
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
            {
                return NotFound(Resources.ContactNotFound);
            }
            if (contact.UserId != CurrentUserId)
            {
                return NotFound(Resources.ContactNotFound);
            }
            var skillLevel = await _context.SkillLevels.FirstOrDefaultAsync(x => x.LevelCode == skillLevelCode);
            var contactSkill = new ContactSkill
            {
                ContactId = contactId,
                SkillId = skillId,
                SkillLevel = skillLevel
            };
            await _context.ContactSkills.AddAsync(contactSkill);
            await _context.SaveChangesAsync();

            return Ok(Resources.ContactSkillCreated);
        }

        /// <summary>
        /// Updates a skill level for a skill of a contact.
        /// </summary>
        /// <param name="skillId"></param> <param name="contactId"></param> <param name="skillLevelCode"></param>
        [HttpPut("/UpdateSkillForContact")]
        public async Task<ActionResult> UpdateSkillForContact(int skillId, int contactId, int skillLevelCode)
        {
            if (!_context.Skills.Any(x => x.Id == skillId))
            {
                return NotFound(Resources.SkillNotFound);
            }
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
            {
                return NotFound(Resources.ContactNotFound);
            }
            if (contact.UserId != CurrentUserId)
            {
                return NotFound(Resources.ContactNotFound);
            }
            if (!_context.SkillLevels.Any(x => x.LevelCode == skillLevelCode))
            {
                return NotFound(Resources.SkillLevelNotFound);
            }
            var existingSkill = await _context.ContactSkills.FirstOrDefaultAsync(x => x.SkillId == skillId && x.ContactId == contactId);
            if (existingSkill == null)
            {
                return NotFound(Resources.ContactSkillNotFound);
            }
            
            
            
            var skillLevel = await _context.SkillLevels.FirstOrDefaultAsync(x => x.LevelCode == skillLevelCode);

            existingSkill.SkillLevel = skillLevel;
            await _context.SaveChangesAsync();

            return Ok(Resources.ContactSkillUpdated);
        }

        /// <summary>
        /// Removes a skill.
        /// </summary>
        /// <param name="id"></param>param>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return NotFound(Resources.SkillNotFound);
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            return Ok(Resources.SkillRemoved);
        }

        private bool SkillExists(int id)
        {
            return _context.Skills.Any(e => e.Id == id);
        }
    }
}
