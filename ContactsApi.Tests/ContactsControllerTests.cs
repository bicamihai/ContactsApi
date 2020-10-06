using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContactsApi.Controllers;
using ContactsApi.Mappings;
using ContactsApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ContactsApi.Tests
{
    public class ContactsControllerTests
    {
        private readonly Mock<ContactsController> _sut;

        private const int DB_CONTACT_FOR_LOGGED_IN_USER = 1;
        private const int DB_CONTACT_FOR_ANOTHER_USER = 2;
        private const int DB_CONTACT_NOT_IN_DATABASE = 3;
        private const string LOGGED_IN_USER_ID = "95d48f31-14df-4a11-bb63-dd64b3d022e7";
        private const string ANOTHER_USER_ID = "e2caf916-873b-40ab-a8fa-778af6493105";
        public ContactsControllerTests()
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())));
            var contactContext = DatabasePreparation.PrepareContactContext(LOGGED_IN_USER_ID, ANOTHER_USER_ID, DB_CONTACT_FOR_LOGGED_IN_USER, DB_CONTACT_FOR_ANOTHER_USER);
            var applicationDbContext = DatabasePreparation.PrepareApplicationDbContext(LOGGED_IN_USER_ID, ANOTHER_USER_ID);
            _sut = new Mock<ContactsController>(contactContext, applicationDbContext, mapper) { CallBase = true };
            _sut.SetupGet(x => x.CurrentUserId).Returns(LOGGED_IN_USER_ID);
        }

        [Fact]
        public async Task GetContacts_ReturnsOnlyLoggedInUserContacts()
        {
            var response = await _sut.Object.GetContacts();
            Assert.Single(response.Value);
            Assert.Equal(response.Value.FirstOrDefault()?.Id, DB_CONTACT_FOR_LOGGED_IN_USER);
        }

        [Fact]
        public async Task GetContactSkills_ReturnsBadRequestIfContactNotFound()
        {
            var response = await _sut.Object.GetContactSkills(DB_CONTACT_NOT_IN_DATABASE);
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task GetContactSkills_ReturnsBadRequestIfContactDoesNotBelongToLoggedInUser()
        {
            var response = await _sut.Object.GetContactSkills(DB_CONTACT_FOR_ANOTHER_USER);
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }
    }
}