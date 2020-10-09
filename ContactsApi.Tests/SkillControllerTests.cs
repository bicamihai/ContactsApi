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
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ContactsApi.Tests
{
    public class SkillControllerTests : IClassFixture<SkillFixture>
    {
        private readonly ContactContext _context;

        private readonly Mock<SkillsController> _sut;

        private readonly SkillModel _skillModel = new SkillModel
        {
            Name = "TestSkill",
            Id = FixtureBase.DrinkingBeerSkillId,
        };

        public SkillControllerTests(SkillFixture fixture)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())));
            _context = fixture.ContactContext;
            _sut = new Mock<SkillsController>(fixture.ContactContext, fixture.ApplicationDbContext, mapper) { CallBase = true };
            _sut.SetupGet(x => x.CurrentUserId).Returns(FixtureBase.LoggedInUserId);
        }

        [Fact]
        public async Task GetSkills_CorrectlyReturnsAllSkillFromDatabase()
        {
            var response = await _sut.Object.GetSkills();

            var resultList = response.Value.Select(x => x.Id).ToList();
            var databaseList = new List<int> { FixtureBase.DrinkingBeerSkillId, FixtureBase.RidingBikeSkillId };
            var areEquivalent = resultList.Count == databaseList.Count && !resultList.Except(databaseList).Any();

            Assert.True(areEquivalent);
        }

        [Fact]
        public async Task GetSkill_ReturnsNotFoundIfSkillNotFound()
        {
            var response = await _sut.Object.GetSkill(FixtureBase.SkillIdNotInDatabase);
            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async Task GetSkill_ReturnsCorrectSkill()
        {
            var response = await _sut.Object.GetSkill(FixtureBase.DrinkingBeerSkillId);
            Assert.Equal(response.Value.Id, FixtureBase.DrinkingBeerSkillId);
        }

        [Fact]
        public async Task PutSkill_ReturnsNotFoundIfSkillIsNotFound()
        {
            var skillModel = new SkillModel()
            {
                Id = FixtureBase.SkillIdNotInDatabase
            };
            var response = await _sut.Object.PutSkill(skillModel);

            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async Task PutSkill_CorrectlyUpdatesFieldsInDatabase()
        {
            var response = await _sut.Object.PutSkill(_skillModel);
            var dbContact = await _context.Skills.FindAsync(FixtureBase.DrinkingBeerSkillId);

            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(_skillModel.Name, dbContact.Name);
        }

        [Fact]
        public async Task PostSkill_CorrectlyAddsSkillInDatabase()
        {
            var response = await _sut.Object.PostSkill(_skillModel);
            var createdSkill = (SkillModel) ((CreatedAtActionResult) response.Result).Value;
            var dbSkill = await _context.Skills.FindAsync(createdSkill.Id);

            Assert.Equal(_skillModel.Name, dbSkill.Name);

            _context.Skills.Remove(dbSkill);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteSkill_CorrectlyRemovesTheSkillFromTheDatabase()
        {
            var dbSkill = new Skill();
            await _context.Skills.AddAsync(dbSkill);
            await _context.SaveChangesAsync();

            var response = await _sut.Object.DeleteSkill(dbSkill.Id);
            Assert.IsType<OkObjectResult>(response);
            Assert.Null(await _context.Skills.FindAsync(dbSkill.Id));
        }

        [Fact]
        public async Task DeleteSkill_ReturnsNotFoundIfSkillNotFound()
        {
            var response = await _sut.Object.DeleteSkill(FixtureBase.SkillIdNotInDatabase);
            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async Task AddSkillToContact_ReturnsBadRequestIfContactSkillAlreadyExists()
        {
            var response = await _sut.Object.AddSkillToContact(FixtureBase.DrinkingBeerSkillId, FixtureBase.ContactIdForLoggedInUser, FixtureBase.NoobSkillLevelCode);
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal(((BadRequestObjectResult)response).Value, Resources.ContactSkillExists);
        }
        [Fact]
        public async Task AddSkillToContact_ReturnsNotFoundIfSkillNotFound()
        {
            var response = await _sut.Object.AddSkillToContact(FixtureBase.SkillIdNotInDatabase, 0, 0);
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal(((NotFoundObjectResult)response).Value, Resources.SkillNotFound);
        }
        [Fact]
        public async Task AddSkillToContact_ReturnsNotFoundIfSkillLevelCodeNotFound()
        {
            var response = await _sut.Object.AddSkillToContact(FixtureBase.RidingBikeSkillId, FixtureBase.ContactIdForLoggedInUser, FixtureBase.NotInDatabaseSkillLevelCode);
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal(((NotFoundObjectResult)response).Value, Resources.SkillLevelNotFound);
        }
        [Fact]
        public async Task AddSkillToContact_ReturnsNotFoundIfContactNotFound()
        {
            var response = await _sut.Object.AddSkillToContact(FixtureBase.RidingBikeSkillId, FixtureBase.ContactIdNotInDatabase, FixtureBase.NoobSkillLevelCode);
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal(((NotFoundObjectResult)response).Value, Resources.ContactNotFound);
        }
        [Fact]
        public async Task AddSkillToContact_ReturnsNotFoundIfContactDoesNotBelongToLoggedInUser()
        {
            var response = await _sut.Object.AddSkillToContact(FixtureBase.RidingBikeSkillId, FixtureBase.ContactIdForNotLoggedInUser, FixtureBase.NoobSkillLevelCode);
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal(((NotFoundObjectResult)response).Value, Resources.ContactNotFound);
        }

        [Fact]
        public async Task AddSkillToContact_CorrectlyAddsContactSkill()
        {
            var response = await _sut.Object.AddSkillToContact(FixtureBase.RidingBikeSkillId, FixtureBase.ContactIdForLoggedInUser, FixtureBase.NoobSkillLevelCode);
            

            var contactSkill = await _context.ContactSkills.Where(x => x.ContactId == FixtureBase.ContactIdForLoggedInUser &&
                                                                      x.SkillId == FixtureBase.RidingBikeSkillId &&
                                                                      x.SkillLevel.LevelCode == FixtureBase.NoobSkillLevelCode).FirstOrDefaultAsync();
            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull(contactSkill);

            _context.Remove(contactSkill);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateSkillForContact_ReturnsNotFoundIfSkillNotFound()
        {
            var response = await _sut.Object.UpdateSkillForContact(FixtureBase.SkillIdNotInDatabase, FixtureBase.ContactIdForLoggedInUser, FixtureBase.NoobSkillLevelCode);
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal(((NotFoundObjectResult)response).Value, Resources.SkillNotFound);
        }

        [Fact]
        public async Task UpdateSkillForContact_ReturnsNotFoundIfContactNotFound()
        {
            var response = await _sut.Object.UpdateSkillForContact(FixtureBase.RidingBikeSkillId, FixtureBase.ContactIdNotInDatabase, FixtureBase.NoobSkillLevelCode);
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal(((NotFoundObjectResult)response).Value, Resources.ContactNotFound);
        }

        [Fact]
        public async Task UpdateSkillForContact_ReturnsNotFoundIfContactDoesNotBelongToLoggedInUser()
        {
            var response = await _sut.Object.UpdateSkillForContact(FixtureBase.RidingBikeSkillId, FixtureBase.ContactIdForNotLoggedInUser, FixtureBase.NoobSkillLevelCode);
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal(((NotFoundObjectResult)response).Value, Resources.ContactNotFound);
        }

        [Fact]
        public async Task UpdateSkillForContact_ReturnsNotFoundIfSkillLevelCodeNotFound()
        {
            var response = await _sut.Object.UpdateSkillForContact(FixtureBase.RidingBikeSkillId, FixtureBase.ContactIdForLoggedInUser, FixtureBase.NotInDatabaseSkillLevelCode);
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal(((NotFoundObjectResult)response).Value, Resources.SkillLevelNotFound);
        }

        [Fact]
        public async Task UpdateSkillForContact_ReturnsNotFoundIfContactSkillDoesNotExist()
        {
            var response = await _sut.Object.UpdateSkillForContact(FixtureBase.RidingBikeSkillId, FixtureBase.ContactIdForLoggedInUser, FixtureBase.NoobSkillLevelCode);
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal(((NotFoundObjectResult)response).Value, Resources.ContactSkillNotFound);
        }

        [Fact]
        public async Task UpdateSkillForContact_CorrectlyUpdatesContactSkill()
        {
            var response = await _sut.Object.UpdateSkillForContact(FixtureBase.DrinkingBeerSkillId, FixtureBase.ContactIdForLoggedInUser, FixtureBase.AdvancedSkillLevelCode);

            var contactSkill = await _context.ContactSkills.Where(x => x.ContactId == FixtureBase.ContactIdForLoggedInUser &&
                                                                      x.SkillId == FixtureBase.DrinkingBeerSkillId &&
                                                                      x.SkillLevel.LevelCode == FixtureBase.AdvancedSkillLevelCode).FirstOrDefaultAsync();
            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull(contactSkill);
            Assert.Equal(contactSkill.SkillLevel.LevelCode, FixtureBase.AdvancedSkillLevelCode);
            _context.Remove(contactSkill);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteSkill_CascadeDeletesAllRelatedContactSkill()
        {
            var dbSkill = new Skill();
            await _context.Skills.AddAsync(dbSkill);
            await _context.ContactSkills.AddAsync(new ContactSkill
            {
                ContactId = FixtureBase.ContactIdForLoggedInUser,
                Skill = dbSkill,
                SkillLevel = new SkillLevel()
            });
            await _context.SaveChangesAsync();

            await _sut.Object.DeleteSkill(dbSkill.Id);

            Assert.Null(_context.ContactSkills.FirstOrDefault(x => x.SkillId == dbSkill.Id && x.ContactId == FixtureBase.ContactIdForLoggedInUser));
        }
    }
}
