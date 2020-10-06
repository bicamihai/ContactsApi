using ContactsApi.Data;
using ContactsApi.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContactsApi.Tests
{
    public class DatabasePreparation
    {
        #region contacts
        public const int DB_CONTACT_FOR_LOGGED_IN_USER = 1;
        public const int DB_CONTACT_FOR_ANOTHER_USER = 2;
        public const int DB_CONTACT_NOT_IN_DATABASE = 3;
        #endregion

        #region users
        public const string LOGGED_IN_USER_ID = "95d48f31-14df-4a11-bb63-dd64b3d022e7";
        public const string ANOTHER_USER_ID = "e2caf916-873b-40ab-a8fa-778af6493105";
        #endregion

        #region skills
        public const int DrinkingBeerSkillId = 1;
        public const int RidingBikeSkillId = 2;
        #endregion
        public static ApplicationDbContext PrepareApplicationDbContext()
        {
            var applicationDbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                                       .UseInMemoryDatabase(databaseName: "ContactDatabase")
                                       .Options;
            var applicationDbContext = new ApplicationDbContext(applicationDbOptions);
            applicationDbContext.Users.Add(new IdentityUser()
            {
                Id = LOGGED_IN_USER_ID
            });
            applicationDbContext.Users.Add(new IdentityUser()
            {
                Id = ANOTHER_USER_ID,
            });
            applicationDbContext.SaveChanges();
            return applicationDbContext;
        }

        public static ContactContext PrepareContactContext()
        {
            var contactOptions = new DbContextOptionsBuilder<ContactContext>()
                                 .UseInMemoryDatabase(databaseName: "ContactDatabase")
                                 .Options;

            var contactContext = new ContactContext(contactOptions);
            contactContext.Contacts.Add(new Contact
            {
                Id = DB_CONTACT_FOR_LOGGED_IN_USER,
                FirstName = "Mihai",
                LastName = "Bica",
                Address = "B-dul N.Titulescu",
                Email = "mihai@yahoo.com",
                MobilePhoneNumber = "+40743123456",
                UserId = LOGGED_IN_USER_ID
            });
            contactContext.Contacts.Add(new Contact
            {
                Id = DB_CONTACT_FOR_ANOTHER_USER,
                FirstName = "Madalina",
                LastName = "Bica",
                Address = "Paris",
                Email = "mihai@yahoo.com",
                MobilePhoneNumber = "+40743654321",
                UserId = ANOTHER_USER_ID
            });
            contactContext.Skills.Add(new Skill
            {
                Id = DrinkingBeerSkillId,
                Name = "DrinkingBeer"
            });
            contactContext.Skills.Add(new Skill
            {
                Id = RidingBikeSkillId,
                Name = "RidingBike"
            });
            var noobSkilLevel = new SkillLevel
            {
                Id = 1,
                LevelDescription = "Noob",
                LevelCode = 1
            };
            contactContext.SkillLevels.Add(noobSkilLevel);
            var intermediateSkillLevel = new SkillLevel
            {
                Id = 2,
                LevelDescription = "Intermediate",
                LevelCode = 2
            };
            contactContext.SkillLevels.Add(intermediateSkillLevel);
            var advancedSkillLevel = new SkillLevel
            {
                Id = 3,
                LevelDescription = "Advanced",
                LevelCode = 3
            };
            contactContext.SkillLevels.Add(advancedSkillLevel);
            contactContext.ContactSkills.Add(new ContactSkill
            {
                SkillId = DrinkingBeerSkillId, 
                ContactId = DB_CONTACT_FOR_LOGGED_IN_USER,
                SkillLevel = noobSkilLevel
            });
            contactContext.ContactSkills.Add(new ContactSkill
            {
                SkillId = RidingBikeSkillId, //RidingBike
                ContactId = DB_CONTACT_FOR_LOGGED_IN_USER,
                SkillLevel = advancedSkillLevel
            });
            contactContext.SaveChanges();


            return contactContext;
        }
    }
}
