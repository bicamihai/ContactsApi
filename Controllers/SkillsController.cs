using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ContactsApi.Data;
using ContactsApi.Data.Models;
using ContactsApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ContactsApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SkillsController : BaseController
    {
        public SkillsController(IContactContext context, IApplicationDbContext applicationDbContext, IMapper mapper) : base(context, applicationDbContext, mapper)
        {
        }

        /// <summary>
        /// Gets a list of all skills in the database.
        /// </summary>
        /// <response code="200">Returns a list of all skills in the database</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SkillModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SkillModel>>> GetSkills()
        {
            var skills = await Context.GetSkillsAsync();
            return new ActionResult<IEnumerable<SkillModel>>(Mapper.Map<IEnumerable<SkillModel>>(skills));
        }

        /// <summary>
        /// Gets a list of all skills levels in the database.
        /// The skill levels are seed data, therefore, they cannot be edited
        /// </summary>
        /// <response code="200">Returns a list of all skill levels in the database</response>
        [HttpGet("/api/GetSkillLevels")]
        [ProducesResponseType(typeof(IEnumerable<SkillLevelModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SkillLevelModel>>> GetSkillLevels()
        {
            var skillLevels = await Context.GetSkillLevelsAsync();
            return new ActionResult<IEnumerable<SkillLevelModel>>(Mapper.Map<IEnumerable<SkillLevelModel>>(skillLevels));
        }

        /// <summary>
        /// Gets a specific skill.
        /// </summary>
        /// <param name="id">The id of the skill</param>
        /// <response code="200">Returns specific skill details.</response>
        /// <response code="404">Skill was not found.</response>
        /// <response code="400">Parameter id not a valid int</response>  
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SkillModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SkillModel>> GetSkill(int id)
        {
            var skill = await Context.GetSkillAsync(id);

            if (skill == null)
            {
                return NotFound(Resources.SkillNotFound);
            }

            return new ActionResult<SkillModel>(Mapper.Map<SkillModel>(skill));
        }

        /// <summary>
        /// Edits the details of a specific skill.
        /// </summary>
        /// <param name="skillModel">Skill model should contain the correct id of the record that needs to be updated</param>
        /// <response code="200">Skill was successfully updated.</response>
        /// <response code="404">Skill was not found.</response>
        /// <response code="400">Validation errors for skill fields</response> 
        [HttpPut]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutSkill(SkillModel skillModel)
        {
            var skill = await Context.GetSkillAsync(skillModel.Id);
            if (skill == null)
            {
                return NotFound(Resources.SkillNotFound);
            }
            Mapper.Map(skillModel, skill);
            try
            {
                await Context.SaveChangesAsync();
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
        /// <param name="skillModel">Skill model to be added. Id property can be ignored on this request as it is handled by the database</param>
        /// <response code="200">Skill was successfully added.</response>
        /// <response code="400">Validation errors for skill fields</response> 
        [HttpPost]
        [ProducesResponseType(typeof(SkillModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SkillModel>> PostSkill(SkillModel skillModel)
        {
            var skill = Mapper.Map<Skill>(skillModel);
            skill.Id = 0;
            await Context.AddSkillAsync(skill);
            await Context.SaveChangesAsync();
            skillModel.Id = skill.Id;
            return CreatedAtAction("GetSkill", new { id = skillModel.Id }, skillModel);
        }

        /// <summary>
        /// Adds a skill with a skill level to a customer.
        /// </summary>
        /// <param name="skillId">The skill</param> <param name="contactId">The contact</param> <param name="skillLevelCode">The code of the skill level</param>
        /// <response code="200">Skill for Contact was successfully added.</response>
        /// <response code="404">Skill, Contact or Skill level not found</response>
        /// <response code="400">The specified skill for the specified contact already exists, use PUT method if you want to update</response>

        [HttpPost("/api/AddSkillToContact")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddSkillToContact(int skillId, int contactId, int skillLevelCode)
        {
            var contactSkill =await  Context.GetContactSkillAsync(contactId, skillId);
            if (contactSkill != null)  
            {
                return BadRequest(Resources.ContactSkillExists);
            }
            var skill = await Context.GetSkillAsync(skillId);
            if (skill == null)
            {
                return NotFound(Resources.SkillNotFound);
            }

            var skillLevel = await Context.GetSkillLevelAsync(skillLevelCode);
            if (skillLevel == null)
            {
                return NotFound(Resources.SkillLevelNotFound);
            }

            var contact = await Context.GetContactAsync(contactId);
            if (contact == null)
            {
                return NotFound(Resources.ContactNotFound);
            }
            if (contact.UserId != CurrentUserId)
            {
                return NotFound(Resources.ContactNotFound);
            }
            contactSkill = new ContactSkill
            {
                ContactId = contactId,
                SkillId = skillId,
                SkillLevel = skillLevel
            };
            await Context.AddContactSkillsAsync(contactSkill);
            await Context.SaveChangesAsync();

            return Ok(Resources.ContactSkillCreated);
        }

        /// <summary>
        /// Updates a skill level for a skill of a contact.
        /// </summary>
        /// <param name="skillId">The skill</param> <param name="contactId">The contact</param> <param name="skillLevelCode">The code of the skill level</param>
        /// <response code="200">Skill for contact was successfully updated.</response>
        /// <response code="404">Skill for Contact or Skil level was not found</response>
        [HttpPut("/api/UpdateSkillForContact")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateSkillForContact(int skillId, int contactId, int skillLevelCode)
        {
            var skill = await Context.GetSkillAsync(skillId);
            if (skill == null)
            {
                return NotFound(Resources.SkillNotFound);
            }
            var contact = await Context.GetContactAsync(contactId);
            if (contact == null)
            {
                return NotFound(Resources.ContactNotFound);
            }
            if (contact.UserId != CurrentUserId)
            {
                return NotFound(Resources.ContactNotFound);
            }

            var skillLevel = await Context.GetSkillLevelAsync(skillLevelCode);
            if (skillLevel == null)
            {
                return NotFound(Resources.SkillLevelNotFound);
            }

            var existingSkill = await Context.GetContactSkillAsync(contactId, skillId);
            if (existingSkill == null)
            {
                return NotFound(Resources.ContactSkillNotFound);
            }
            existingSkill.SkillLevel = skillLevel;
            await Context.SaveChangesAsync();

            return Ok(Resources.ContactSkillUpdated);
        }

        /// <summary>
        /// Removes a skill.
        /// </summary>
        /// <param name="id">Id of the skill to be removed</param>
        /// <response code="200">Skill was successfully removed</response>
        /// <response code="404">Skill was not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteSkill(int id)
        {
            var skill = await Context.GetSkillAsync(id);
            if (skill == null)
            {
                return NotFound(Resources.SkillNotFound);
            }

            Context.Remove<Skill>(skill);
            await Context.SaveChangesAsync();

            return Ok(Resources.SkillRemoved);
        }

        private bool SkillExists(int id)
        {
            return Context.GetSkill(id) != null; 
        }
    }
}
