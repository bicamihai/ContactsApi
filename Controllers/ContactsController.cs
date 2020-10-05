using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactsApi.Data;
using ContactsApi.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactsApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace ContactsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactsController : BaseController
    {
        private readonly ContactContext _context;
        

        public ContactsController(ContactContext context, ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _context = context;
        } 
        

        // GET: api/Contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            var userId = GetCurrentUser();
            return await _context.Contacts.Where(c => c.UserId == userId).ToListAsync();
        }

        // GET: api/Contacts
        [HttpGet("/GetContactSkills")]
        public async Task<ActionResult<Dictionary<string, string>>> GetContactSkills(int contactId)
        {
            var userId = GetCurrentUser();
            var contact = await _context.Contacts.Where(c=>c.Id == contactId && c.UserId == userId)
                                        .Include(x=>x.ContactSkills)
                                        .ThenInclude(x=>x.Skill)
                                        .FirstOrDefaultAsync();
            if (contact == null)
            {
                return BadRequest("Invalid contact");
            }

            var returnList = new Dictionary<string, string>();
            contact.ContactSkills.ToList().ForEach(contactSkill =>
            {
                returnList.Add(contactSkill.Skill.Name, contactSkill.SkillLevel.ToString());
            });
            return returnList;
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        // PUT: api/Contacts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(int id, Contact contact)
        {
            if (id != contact.Id)
            {
                return BadRequest();
            }

            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
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

        // POST: api/Contacts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact(Contact contact)
        {
            contact.UserId = GetCurrentUser();
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContact", new { id = contact.Id }, contact);
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Contact>> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return contact;
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }
    }
}
