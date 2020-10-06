using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContactsApi.Controllers;
using ContactsApi.Data;
using ContactsApi.Data.Models;
using ContactsApi.Mappings;
using ContactsApi.Models;
using ContactsApi.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ContactsApi.Tests
{
    public class ContactsControllerTests : IClassFixture<ContactFixture>
    {
        private readonly Mock<ContactsController> _sut;

        private readonly ContactContext _context;

        private readonly ContactModel _contactModel = new ContactModel
        {
            Id = ContactFixture.ContactIdForLoggedInUser,
            Address = "new Address",
            Email = "newEmail@gmail.com",
            FirstName = "NewFirstName",
            FullName = "FullName",
            MobilePhoneNumber = "123123",
            LastName = "NewLastName"
        };

        public ContactsControllerTests(ContactFixture fixture)
        {
            _context = fixture.ContactContext;
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())));
            _sut = new Mock<ContactsController>(fixture.ContactContext, fixture.ApplicationDbContext, mapper) { CallBase = true };
            _sut.SetupGet(x => x.CurrentUserId)
                .Returns(FixtureBase.LoggedInUserId);
        }

        [Fact]
        public async Task GetContacts_ReturnsOnlyLoggedInUserContacts()
        {
            var response = await _sut.Object.GetContacts();
            Assert.Single(response.Value);
            Assert.Equal(response.Value.FirstOrDefault()
                                 ?.Id,
                         FixtureBase.ContactIdForLoggedInUser);
        }

        [Fact]
        public async Task GetContactSkills_ReturnsNotFoundIfContactNotFound()
        {
            var response = await _sut.Object.GetContactSkills(FixtureBase.ContactIdNotInDatabase);
            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async Task GetContactSkills_ReturnsNotFoundIfContactDoesNotBelongToLoggedInUser()
        {
            var response = await _sut.Object.GetContactSkills(FixtureBase.ContactIdForNotLoggedInUser);
            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async Task GetContactSkills_ReturnsCorrectSkillsForContact()
        {
            var response = await _sut.Object.GetContactSkills(FixtureBase.ContactIdForLoggedInUser);
            var resultList = response.Value.Select(x => x.SkillId)
                                     .ToList();
            var databaseList = new List<int> { FixtureBase.DrinkingBeerSkillId, FixtureBase.RidingBikeSkillId };

            var areEquivalent = resultList.Count == databaseList.Count &&
                !resultList.Except(databaseList)
                           .Any();
            Assert.True(areEquivalent);
        }

        [Fact]
        public async Task GetContact_ReturnsNotFoundIfContactNotFound()
        {
            var response = await _sut.Object.GetContact(FixtureBase.ContactIdNotInDatabase);
            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async Task GetContact_ReturnsNotFoundIfContactDoesNotBelongToLoggedInUser()
        {
            var response = await _sut.Object.GetContact(FixtureBase.ContactIdNotInDatabase);
            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async Task GetContact_ReturnsCorrectContact()
        {
            var response = await _sut.Object.GetContact(FixtureBase.ContactIdForLoggedInUser);
            Assert.Equal(response.Value.Id, FixtureBase.ContactIdForLoggedInUser);
        }

        [Fact]
        public async Task PutContact_ReturnsNotFoundIfContactIsNotFound()
        {
            var contactModel = new ContactModel
            {
                Id = FixtureBase.ContactIdNotInDatabase
            };
            var response = await _sut.Object.PutContact(contactModel);
            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async Task PutContact_ReturnsNotFoundIfContactDoesNotBelongToLoggedInUser()
        {
            var contactModel = new ContactModel
            {
                Id = FixtureBase.ContactIdForNotLoggedInUser
            };
            var response = await _sut.Object.PutContact(contactModel);
            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async Task PutContact_CorrectlyUpdatesFieldsInDatabase()
        {
            var response = await _sut.Object.PutContact(_contactModel);
            var dbContact = await _context.Contacts.FindAsync(FixtureBase.ContactIdForLoggedInUser);

            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(_contactModel.Address, dbContact.Address);
            Assert.Equal(_contactModel.Email, dbContact.Email);
            Assert.Equal(_contactModel.FirstName, dbContact.FirstName);
            Assert.Equal(_contactModel.FullName, dbContact.FullName);
            Assert.Equal(_contactModel.MobilePhoneNumber, dbContact.MobilePhoneNumber);
            Assert.Equal(_contactModel.LastName, dbContact.LastName);
        }

        [Fact]
        public async Task PostContact_CorrectlyAddsContactInDatabase()
        {
            var response = await _sut.Object.PostContact(_contactModel);
            var createdContact = (ContactModel) ((CreatedAtActionResult) response.Result).Value;
            var dbContact = await _context.Contacts.FindAsync(createdContact.Id);

            Assert.Equal(_contactModel.Address, dbContact.Address);
            Assert.Equal(_contactModel.Email, dbContact.Email);
            Assert.Equal(_contactModel.FirstName, dbContact.FirstName);
            Assert.Equal(_contactModel.FullName, dbContact.FullName);
            Assert.Equal(_contactModel.MobilePhoneNumber, dbContact.MobilePhoneNumber);
            Assert.Equal(_contactModel.LastName, dbContact.LastName);

            _context.Contacts.Remove(dbContact);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task PostContact_CorrectlySetsTheCurrentLoggedInUserId()
        {
            var response = await _sut.Object.PostContact(_contactModel);
            var createdContact = (ContactModel) ((CreatedAtActionResult) response.Result).Value;
            var dbContact = await _context.Contacts.FindAsync(createdContact.Id);

            Assert.Equal(dbContact.UserId, FixtureBase.LoggedInUserId);

            _context.Contacts.Remove(dbContact);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteContact_CorrectlyRemovesTheContactFromTheDatabase()
        {
            var dbContact = new Contact();
            await _context.Contacts.AddAsync(dbContact);
            await _context.SaveChangesAsync();

            var response = await _sut.Object.DeleteContact(dbContact.Id);
            Assert.IsType<OkObjectResult>(response);
            Assert.Null(await _context.Contacts.FindAsync(dbContact.Id));
        }

        [Fact]
        public async Task DeleteContact_ReturnsNotFoundIfContactNotFound()
        {
            var response = await _sut.Object.DeleteContact(FixtureBase.ContactIdNotInDatabase);
            Assert.IsType<NotFoundObjectResult>(response);
        }
    }
}