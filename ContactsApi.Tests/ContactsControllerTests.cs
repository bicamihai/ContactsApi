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
    public class ContactsControllerTests : IClassFixture<DbContextFixture>
    {
        private readonly Mock<ContactsController> _sut;
        public ContactsControllerTests(DbContextFixture fixture)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())));
            var contactContext = fixture.ContactContext;
            var applicationDbContext = fixture.ApplicationDbContext;
            _sut = new Mock<ContactsController>(contactContext, applicationDbContext, mapper) { CallBase = true };
            _sut.SetupGet(x => x.CurrentUserId).Returns(DbContextFixture.LoggedInUserId);
        }

        [Fact]
        public async Task GetContacts_ReturnsOnlyLoggedInUserContacts()
        {
            var response = await _sut.Object.GetContacts();
            Assert.Single(response.Value);
            Assert.Equal(response.Value.FirstOrDefault()?.Id, DbContextFixture.ContactIdForLoggedInUser);
        }

        [Fact]
        public async Task GetContactSkills_ReturnsBadRequestIfContactNotFound()
        {
            var response = await _sut.Object.GetContactSkills(DbContextFixture.ContactIdNotInDatabase);
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task GetContactSkills_ReturnsBadRequestIfContactDoesNotBelongToLoggedInUser()
        {
            var response = await _sut.Object.GetContactSkills(DbContextFixture.ContactIdForNotLoggedInUSer);
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task GetContactSkills_ReturnsCorrectSkillsForContact()
        {
            var response = await _sut.Object.GetContactSkills(DbContextFixture.ContactIdForLoggedInUser);
            var resultList = response.Value.Select(x => x.SkillId).ToList();
            var databaseList = new List<int> { DbContextFixture.DrinkingBeerSkillId, DbContextFixture.RidingBikeSkillId };

            var areEquivalent = resultList.Count == databaseList.Count && !resultList.Except(databaseList).Any();
            Assert.True(areEquivalent);
        }
    }
}