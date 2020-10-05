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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactModel>>> GetContacts()
        {
            var contacts = await _context.Contacts.Where(c => c.UserId == CurrentUserId).ToListAsync();
            var contactsDto = Mapper.Map<IEnumerable<ContactModel>>(contacts);
            return new ActionResult<IEnumerable<ContactModel>>(contactsDto);
        }

        [HttpGet("/GetContactSkills")]
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
                return BadRequest(Resources.ContactNotFound);
            }

            var returnList = Mapper.Map<IEnumerable<ContactSkillModel>>(contact.ContactSkills);
            return new ActionResult<IEnumerable<ContactSkillModel>>(returnList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactModel>> GetContact(int id)
        {
            var contact = await _context.Contacts.FirstOrDefaultAsync(c=>c.Id == id && c.UserId == CurrentUserId);
            if (contact == null)
            {
                return NotFound(Resources.ContactNotFound);
            }
            return new ActionResult<ContactModel>(Mapper.Map<ContactModel>(contact));
        }

        [HttpPut]
        public async Task<IActionResult> PutContact(ContactModel contactModel)
        {
            var contact = await _context.Contacts.FindAsync(contactModel.Id);
            if (contact == null || contact.UserId != CurrentUserId)
            {
                return BadRequest(Resources.ContactNotFound);
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

        [HttpPost]
        public async Task<ActionResult<ContactModel>> PostContact(ContactModel contactModel)
        {
            var contact = Mapper.Map<Contact>(contactModel);
            contact.Id = 0;
            contact.UserId = CurrentUserId;
            await _context.Contacts.AddAsync(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContact", new { id = contact.Id }, contactModel);
        }

        [HttpDelete("{id}")]
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
