﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContactsApi.Controllers;
using ContactsApi.Data;
using ContactsApi.Mappings;
using ContactsApi.Models;
using ContactsApi.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc;
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
            SkillCode = 9
        };

        public SkillControllerTests(SkillFixture fixture)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())));
            _context = fixture.ContactContext;
            _sut = new Mock<SkillsController>(fixture.ContactContext, fixture.ApplicationDbContext, mapper) { CallBase = true };
        }

        [Fact]
        public async Task GetSkills_CorrectlyReturnsAllSkillFromDatabase()
        {
            var response = await _sut.Object.GetSkills();
            var resultList = response.Value.Select(x => x.Id)
                                     .ToList();
            var databaseList = new List<int> { FixtureBase.DrinkingBeerSkillId, FixtureBase.RidingBikeSkillId };

            var areEquivalent = resultList.Count == databaseList.Count &&
                !resultList.Except(databaseList)
                           .Any();
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
        public async Task PutContact_CorrectlyUpdatesFieldsInDatabase()
        {
            var response = await _sut.Object.PutSkill(_skillModel);
            var dbContact = await _context.Skills.FindAsync(FixtureBase.DrinkingBeerSkillId);

            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(_skillModel.Name, dbContact.Name);
            Assert.Equal(_skillModel.SkillCode, dbContact.SkillCode);
        }
    }
}
