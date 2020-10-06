using ContactsApi.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactsApi.Tests.Fixtures
{
    public class ContactFixture :FixtureBase
    {
        public ContactFixture()
        {
            var applicationDbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                                       .UseInMemoryDatabase(databaseName : "ContactDatabase")
                                       .Options;
            ApplicationDbContext = new ApplicationDbContext(applicationDbOptions);
            var contactOptions = new DbContextOptionsBuilder<ContactContext>()
                                 .UseInMemoryDatabase(databaseName : "ContactDatabase")
                                 .Options;
            ContactContext = new ContactContext(contactOptions);
            SeedTestDb();
        }
    }
}
