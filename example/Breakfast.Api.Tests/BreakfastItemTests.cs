using System.Collections.Generic;
using System.Threading.Tasks;
using Breakfast.Api.Data;
using Xunit.Fixture.Mvc;
using Xunit.Abstractions;
using Breakfast.Api.Entities;
using Breakfast.Api.Models;
using Xunit;
using Xunit.Fixture.Mvc.Extensions;
using Xunit.Fixture.Mvc.EntityFramework.Extensions;

namespace Breakfast.Api.Tests
{
    public abstract class BreakfastItemTestsBase : MvcFunctionalTestBase<Startup>
    {
        private const string BreakfastItem = "breakfastitem";

        private readonly string _environment;

        protected BreakfastItemTestsBase(MvcFunctionalTestClassFixture<Startup> fixture,
            ITestOutputHelper output,
            string environment) : base(fixture, output)
        {
            _environment = environment;
        }

        protected override IMvcFunctionalTestFixture GivenSetup(IMvcFunctionalTestFixture fixture) =>
            fixture
                .HavingAspNetEnvironment(_environment)
                .HavingDatabase<BreakfastContext>()
                .HavingCleanDatabase()
                .HavingPathBase("api");

        [Fact]
        public Task When_getting_all_existing_breakfast_items() =>
            GivenClassFixture()
                .HavingEntities(out ICollection<BreakfastItem> items)
                .WhenGetting(BreakfastItem)
                .ShouldReturnSuccessfulStatus()
                .ShouldReturnJsonCollectionContainingEquivalentModels(items)
                .RunAsync();

        [Fact]
        public Task When_getting_existing_breakfast_item() =>
            GivenClassFixture()
                .HavingEntity(out BreakfastItem item)
                .WhenGettingById(BreakfastItem, item.Id)
                .ShouldReturnSuccessfulStatus()
                .ShouldReturnEquivalentJson(item)
                .RunAsync();

        [Fact]
        public Task When_creating_breakfast_item() =>
            GivenClassFixture()
                .WhenCreating(BreakfastItem, out CreateOrUpdateBreakfastItemRequest request)
                .ShouldReturnSuccessfulStatus()
                .ShouldReturnEquivalentJson<BreakfastItem, CreateOrUpdateBreakfastItemRequest>(request)
                .ShouldSatisfyDatabaseQueryFromJsonResponse<BreakfastItem, BreakfastItem>((r, f) =>
                    f.Find(r.Id).ShouldReturnEquivalent(request))
                .RunAsync();

        [Fact]
        public Task When_updating_breakfast_item() =>
            GivenClassFixture()
                .HavingEntity(out BreakfastItem item)
                .WhenUpdating(BreakfastItem, item.Id, out CreateOrUpdateBreakfastItemRequest request)
                .ShouldReturnSuccessfulStatus()
                .ShouldReturnEquivalentJson<BreakfastItem, CreateOrUpdateBreakfastItemRequest>(request)
                .ShouldBeEquivalentInDatabase<BreakfastItem>(item.Id, request)
                .RunAsync();

        [Fact]
        public Task When_patching_breakfast_item() =>
            GivenClassFixture()
               .HavingEntity(out BreakfastItem item)
               .HavingModel(out CreateOrUpdateBreakfastItemRequest request, (f, r) => r.Rating = null) // do not patch rating.
               .WhenPatching(BreakfastItem, item.Id, request)
               .ShouldReturnSuccessfulStatus()
               .ShouldReturnEquivalentJson(new { item.Id, item.Rating, request.Name })
               .ShouldBeEquivalentInDatabase<BreakfastItem>(item.Id, new { item.Id, item.Rating, request.Name })
               .RunAsync();

        [Fact]
        public Task When_deleting_breakfast_item() =>
            GivenClassFixture()
                .HavingEntity(out BreakfastItem item)
                .WhenDeleting(BreakfastItem, item.Id)
                .ShouldReturnSuccessfulStatus()
                .ShouldNotExistInDatabase<BreakfastItem>(item.Id)
                .RunAsync();
    }

    public class BreakfastItemTests : BreakfastItemTestsBase
    {
        
        public BreakfastItemTests(MvcFunctionalTestClassFixture<Startup> fixture, ITestOutputHelper output)
            : base(fixture, output, "Tests1")
        {
        }
    }

    public class BreakfastItemTests2 : BreakfastItemTestsBase
    {

        public BreakfastItemTests2(MvcFunctionalTestClassFixture<Startup> fixture, ITestOutputHelper output)
            : base(fixture, output, "Tests2")
        {
        }
    }
}
