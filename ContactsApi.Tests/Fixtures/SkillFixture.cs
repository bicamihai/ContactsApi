using ContactsApi.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactsApi.Tests.Fixtures
{
    public class SkillFixture : FixtureBase
    {
        public SkillFixture()
        {
            var applicationDbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                                       .UseInMemoryDatabase(databaseName: "SkillDatabase")
                                       .Options;
            ApplicationDbContext = new ApplicationDbContext(applicationDbOptions);
            var contactOptions = new DbContextOptionsBuilder<ContactContext>()
                                 .UseInMemoryDatabase(databaseName: "SkillDatabase")
                                 .Options;
            ContactContext = new ContactContext(contactOptions);
            SeedTestDb();
        }
    }
}
