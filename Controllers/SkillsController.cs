﻿using System.Collections.Generic;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SkillModel>>> GetSkills()
        {
            var skills = await _context.Skills.ToListAsync();
            return new ActionResult<IEnumerable<SkillModel>>(Mapper.Map<IEnumerable<SkillModel>>(skills));
        }

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

        [HttpPost]
        public async Task<ActionResult<SkillModel>> PostSkill(SkillModel skillModel)
        {
            var skill = Mapper.Map<Skill>(skillModel);
            skill.Id = 0;
            await _context.Skills.AddAsync(skill);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSkill", new { id = skill.Id }, skillModel);
        }

        [HttpPost("/AddSkillToContact")]
        public async Task<ActionResult> AddSkillToContact(int skillId, int contactId, int skillLevelCode)
        {
            if (_context.ContactSkills.Any(x => x.SkillId == skillId && x.ContactId == contactId))
            {
                return BadRequest(Resources.ContactSkillExists);
            }
            if (!_context.Skills.Any(x => x.Id == skillId))
            {
                return BadRequest(Resources.SkillNotFound);
            }
            if (!_context.SkillLevels.Any(x => x.LevelCode == skillLevelCode))
            {
                return BadRequest(Resources.SkillLevelNotFound);
            }
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
            {
                return BadRequest(Resources.ContactNotFound);
            }
            if (contact.UserId != CurrentUserId)
            {
                return BadRequest(Resources.ContactNotFound);
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

        [HttpPut("/UpdateSkillForContact")]
        public async Task<ActionResult> UpdateSkillForContact(int skillId, int contactId, int skillLevelCode)
        {
            var existingSkill = await _context.ContactSkills.FirstOrDefaultAsync(x => x.SkillId == skillId && x.ContactId == contactId);
            if (existingSkill == null)
            {
                return BadRequest(Resources.ContactSkillNotFound);
            }
            if (!_context.Skills.Any(x => x.Id == skillId))
            {
                return BadRequest(Resources.SkillNotFound);
            }
            if (!_context.SkillLevels.Any(x => x.LevelCode == skillLevelCode))
            {
                return BadRequest(Resources.SkillLevelNotFound);
            }
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
            {
                return BadRequest(Resources.ContactNotFound);
            }
            if (contact.UserId != CurrentUserId)
            {
                return BadRequest(Resources.ContactNotFound);
            }
            var skillLevel = await _context.SkillLevels.FirstOrDefaultAsync(x => x.LevelCode == skillLevelCode);

            existingSkill.SkillLevel = skillLevel;
            await _context.SaveChangesAsync();

            return Ok(Resources.ContactSkillUpdated);
        }

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
