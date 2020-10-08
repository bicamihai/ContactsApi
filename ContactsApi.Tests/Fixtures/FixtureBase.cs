using System;
using ContactsApi.Data;
using ContactsApi.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace ContactsApi.Tests.Fixtures
{
    public class FixtureBase : IDisposable
    {
        #region contacts
        public const int ContactIdForLoggedInUser = 1;
        public const int ContactIdForNotLoggedInUser = 2;
        public const int ContactIdNotInDatabase = 3;
        #endregion

        #region users
        public const string LoggedInUserId = "95d48f31-14df-4a11-bb63-dd64b3d022e7";
        public const string AnotherUserId = "e2caf916-873b-40ab-a8fa-778af6493105";
        #endregion

        #region skills
        public const int DrinkingBeerSkillId = 10;
        public const int RidingBikeSkillId = 12;
        internal static readonly int SkillIdNotInDatabase = 13;
        #endregion

        #region skill level
        public const int NoobSkillLevelCode = 111;
        public const int IntermediateSkillLevelCode = 112;
        public const int AdvancedSkillLevelCode = 113;
        public const int NotInDatabaseSkillLevelCode = 1110;
        #endregion

        public ApplicationDbContext ApplicationDbContext { get; set; }
        public ContactContext ContactContext { get; set; }

        

        public void SeedTestDb()
        {
            ApplicationDbContext.Users.Add(new IdentityUser()
            {
                Id = LoggedInUserId
            });
            ApplicationDbContext.Users.Add(new IdentityUser()
            {
                Id = AnotherUserId,
            });
            ApplicationDbContext.SaveChanges();
            ContactContext.Contacts.Add(new Contact
            {
                Id = ContactIdForLoggedInUser,
                FirstName = "Mihai",
                LastName = "Bica",
                Address = "B-dul N.Titulescu",
                Email = "mihai@yahoo.com",
                MobilePhoneNumber = "+40743123456",
                UserId = LoggedInUserId
            });
            ContactContext.Contacts.Add(new Contact
            {
                Id = ContactIdForNotLoggedInUser,
                FirstName = "Madalina",
                LastName = "Bica",
                Address = "Paris",
                Email = "mihai@yahoo.com",
                MobilePhoneNumber = "+40743654321",
                UserId = AnotherUserId
            });
            ContactContext.Skills.Add(new Skill
            {
                Id = DrinkingBeerSkillId,
                Name = "DrinkingBeer"
            });
            ContactContext.Skills.Add(new Skill
            {
                Id = RidingBikeSkillId,
                Name = "RidingBike"
            });
            var noobSkilLevel = new SkillLevel
            {
                Id = 1,
                LevelDescription = "Noob",
                LevelCode = NoobSkillLevelCode
            };
            ContactContext.SkillLevels.Add(noobSkilLevel);
            var intermediateSkillLevel = new SkillLevel
            {
                Id = 2,
                LevelDescription = "Intermediate",
                LevelCode = IntermediateSkillLevelCode
            };
            ContactContext.SkillLevels.Add(intermediateSkillLevel);
            var advancedSkillLevel = new SkillLevel
            {
                Id = 3,
                LevelDescription = "Advanced",
                LevelCode = AdvancedSkillLevelCode
            };
            ContactContext.SkillLevels.Add(advancedSkillLevel);
            ContactContext.ContactSkills.Add(new ContactSkill
            {
                SkillId = DrinkingBeerSkillId,
                ContactId = ContactIdForLoggedInUser,
                SkillLevel = noobSkilLevel
            });
            ContactContext.SaveChanges();
        }
        public void Dispose()
        {
            ApplicationDbContext.Database.EnsureDeleted();
            ContactContext.Database.EnsureDeleted();
            ApplicationDbContext.Dispose();
            ContactContext.Dispose();
        }
    }
}
