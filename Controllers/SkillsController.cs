using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactsApi.Data;
using ContactsApi.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ContactsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SkillsController : BaseController
    {
        private readonly ContactContext _context;

        public SkillsController(ContactContext context, ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            return await _context.Skills.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Skill>> GetSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound();
            }

            return skill;
        }

        // PUT: api/Skills/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSkill(int id, Skill skill)
        {
            if (id != skill.Id)
            {
                return BadRequest();
            }

            _context.Entry(skill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Skills
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Skill>> PostSkill(Skill skill)
        {

            await _context.Skills.AddAsync(skill);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSkill", new { id = skill.Id }, skill);
        }

        [HttpPost("/AddSkillToContact")]
        public async Task<ActionResult<Skill>> AddSkillToContact(int skillId, int contactId, int skillLevel)
        {
            if (!_context.Skills.Any(x => x.Id == skillId))
            {
                return BadRequest("Invalid skill");
            }
            if (!_context.SkillLevels.Any(x => x.LevelCode == skillLevel))
            {
                return BadRequest("Invalid skill level");
            }
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
            {
                return BadRequest("Invalid user");
            }
            if (contact.UserId != CurrentUserId)
            {
                return BadRequest("Invalid user");
            }
            var skillLevelData = await _context.SkillLevels.FirstOrDefaultAsync(x => x.LevelCode == skillLevel);
            var contactSkill = new ContactSkill
            {
                ContactId = contactId,
                SkillId = skillId,
                SkillLevel = skillLevelData
            };
            await _context.ContactSkills.AddAsync(contactSkill);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSkill", new { id = contactSkill.ContactId }, skillLevel);
        }

        // DELETE: api/Skills/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Skill>> DeleteSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return NotFound();
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            return skill;
        }

        private bool SkillExists(int id)
        {
            return _context.Skills.Any(e => e.Id == id);
        }
    }
}
