using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactsApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactsApi.Models;
using ContactsApi.Models.Enums;
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

        // GET: api/Skills
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            return await _context.Skills.ToListAsync();
        }

        // GET: api/Skills/5
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

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSkill", new { id = skill.Id }, skill);
        }

        [HttpPost("/addskilltocontact")]
        public async Task<ActionResult<Skill>> AddSkillToContact(int skillId, int contactId, int skillLevel)
        {
            if (skillLevel < 0 || skillLevel > 2)
            {
                return BadRequest("Invalid skill level");
            }

            if (!_context.Skills.Any(x => x.Id == skillId))
            {
                return BadRequest("Invalid skill");
            }
           
            var user = GetCurrentUser();
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
            {
                return BadRequest("Invalid user");
            }

            if (contact.UserId != user)
            {
                return BadRequest("Invalid user");
            }

            var contactSkill = new ContactSkill
            {
                ContactId = contactId,
                SkillId = skillId,
                SkillLevel = (SkillEnum) skillLevel
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
