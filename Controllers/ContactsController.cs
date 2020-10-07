using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using ContactsApi.Data;
using ContactsApi.Data.Models;
using ContactsApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Swashbuckle.Swagger.Annotations;

namespace ContactsApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactsController : BaseController
    {
        private readonly ContactContext _context;

        public ContactsController(ContactContext context, ApplicationDbContext applicationDbContext, IMapper mapper) : base(applicationDbContext, mapper)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all contacts of signed in user.
        /// </summary>
        /// <response code="200">Returns all contacts of signed in user</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContactModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContactModel>>> GetContacts()
        {
            var contacts = await _context.Contacts.Where(c => c.UserId == CurrentUserId).ToListAsync();
            var contactsDto = Mapper.Map<IEnumerable<ContactModel>>(contacts);
            return new ActionResult<IEnumerable<ContactModel>>(contactsDto);
        }

        /// <summary>
        /// Gets all skills with the specific skill level for the specified contact.
        /// </summary>
        /// <param name="contactId">the contact</param>
        /// <response code="200">Returns all skills with specific skill level for the specified customer.</response>
        /// <response code="404">Contact was not found.</response>
        /// <response code="400">Parameter contactId not a valid int</response> 
        [HttpGet("/GetContactSkills")]
        [ProducesResponseType(typeof(IEnumerable<ContactSkillModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ContactSkillModel>>> GetContactSkills(int contactId)
        {
            var contact = await _context.Contacts.Where(c=>c.Id == contactId && c.UserId == CurrentUserId)
                                        .Include(x=>x.ContactSkills)
                                        .ThenInclude(x=>x.Skill)
                                        .Include(x => x.ContactSkills)
                                        .ThenInclude(x => x.SkillLevel)
                                        .FirstOrDefaultAsync();
            if (contact == null)
            {
                return NotFound(Resources.ContactNotFound);
            }

            var returnList = Mapper.Map<IEnumerable<ContactSkillModel>>(contact.ContactSkills);
            return new ActionResult<IEnumerable<ContactSkillModel>>(returnList);
        }

        /// <summary>
        /// Gets a specific contact.
        /// </summary>
        /// <param name="id">the contact</param>
        /// <response code="200">Returns contact details.</response>
        /// <response code="404">Contact was not found.</response>
        /// <response code="400">Parameter contactId not a valid int</response> 
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ContactModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactModel>> GetContact(int id)
        {
            var contact = await _context.Contacts.FirstOrDefaultAsync(c=>c.Id == id && c.UserId == CurrentUserId);
            if (contact == null)
            {
                return NotFound(Resources.ContactNotFound);
            }
            return new ActionResult<ContactModel>(Mapper.Map<ContactModel>(contact));
        }

        /// <summary>
        /// Edits details of a specific customer.
        /// </summary>
        /// <param name="contactModel">Contact model should contain the correct id of the record that needs to be updated</param>
        /// <response code="200">Contact was successfully updated.</response>
        /// <response code="404">Contact was not found.</response>
        /// <response code="400">Validation errors for contact fields</response> 
        [HttpPut]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutContact(ContactModel contactModel)
        {
            var contact = await _context.Contacts.FindAsync(contactModel.Id);
            if (contact == null || contact.UserId != CurrentUserId)
            {
                return NotFound(Resources.ContactNotFound);
            }

            Mapper.Map(contactModel, contact);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(contactModel.Id))
                {
                    return NotFound(Resources.ContactNotFound);
                }
                throw;
            }
            return Ok(Resources.ContactUpdated);
        }

        /// <summary>
        /// Adds a customer for the signed in user.
        /// </summary>
        /// <param name="contactModel">The contactModel to be added, id property is ignored, as it is handled by the database</param>
        /// <response code="200">Contact was successfully added.</response>
        /// <response code="400">Validation errors for contact fields</response> 
        [HttpPost]
        [ProducesResponseType(typeof(ContactModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactModel>> PostContact(ContactModel contactModel)
        {
            var contact = Mapper.Map<Contact>(contactModel);
            contact.Id = 0;
            contact.UserId = CurrentUserId;
            await _context.Contacts.AddAsync(contact);
            await _context.SaveChangesAsync();
            contactModel.Id = contact.Id;
            return CreatedAtAction("GetContact", new { id = contactModel.Id }, contactModel);
        }

        /// <summary>
        /// Deletes a specific Contact.
        /// </summary>
        /// <param name="id">The contact to be removed</param>
        /// <response code="200">Contact was successfully updated.</response>
        /// <response code="404">Contact was not found.</response>
        /// <response code="400">Parameter contactId not a valid int</response> 
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound(Resources.ContactNotFound);
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return Ok(Resources.ContactRemoved);
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }
    }
}
