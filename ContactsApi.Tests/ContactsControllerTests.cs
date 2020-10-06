using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContactsApi.Controllers;
using ContactsApi.Mappings;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ContactsApi.Tests
{
    public class ContactsControllerTests
    {
        private readonly Mock<ContactsController> _sut;
        public ContactsControllerTests()
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())));
            var contactContext = DatabasePreparation.PrepareContactContext();
            var applicationDbContext = DatabasePreparation.PrepareApplicationDbContext();
            _sut = new Mock<ContactsController>(contactContext, applicationDbContext, mapper) { CallBase = true };
            _sut.SetupGet(x => x.CurrentUserId).Returns(DatabasePreparation.LOGGED_IN_USER_ID);
        }

        [Fact]
        public async Task GetContacts_ReturnsOnlyLoggedInUserContacts()
        {
            var response = await _sut.Object.GetContacts();
            Assert.Single(response.Value);
            Assert.Equal(response.Value.FirstOrDefault()?.Id, DatabasePreparation.DB_CONTACT_FOR_LOGGED_IN_USER);
        }

        [Fact]
        public async Task GetContactSkills_ReturnsBadRequestIfContactNotFound()
        {
            var response = await _sut.Object.GetContactSkills(DatabasePreparation.DB_CONTACT_NOT_IN_DATABASE);
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task GetContactSkills_ReturnsBadRequestIfContactDoesNotBelongToLoggedInUser()
        {
            var response = await _sut.Object.GetContactSkills(DatabasePreparation.DB_CONTACT_FOR_ANOTHER_USER);
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task GetContactSkills_ReturnsCorrectSkillsForContact()
        {
            var response = await _sut.Object.GetContactSkills(DatabasePreparation.DB_CONTACT_FOR_LOGGED_IN_USER);
            var resultList = response.Value.Select(x => x.SkillId).ToList();
            var databaseList = new List<int> { DatabasePreparation.DrinkingBeerSkillId, DatabasePreparation.RidingBikeSkillId };

            var areEquivalent = resultList.Count == databaseList.Count && !resultList.Except(databaseList).Any();
            Assert.True(areEquivalent);
        }
    }
}