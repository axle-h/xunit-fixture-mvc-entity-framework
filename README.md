[![CircleCI](https://circleci.com/gh/axle-h/xunit-fixture-mvc-entity-framework/tree/master.svg?style=svg)](https://circleci.com/gh/axle-h/xunit-fixture-mvc-entity-framework/tree/master)
[![NuGet](https://img.shields.io/nuget/v/xunit.fixture.mvc.entityframework.svg)](https://www.nuget.org/packages/xunit.fixture.mvc.entityframework)

# xunit-fixture-mvc-entity-framework

MVC functional tests with a fixture pattern built on top of [xunit-fixture-mvc](https://github.com/axle-h/xunit-fixture-mvc) and extended for a an EF Core database.

For example:

```C#
[Fact]
public Task When_creating_breakfast_item() =>
    new MvcFunctionalTestFixture<Startup>(_output)
        .HavingDatabase<BreakfastContext>()
        .WhenCreating("BreakfastItem", out CreateOrUpdateBreakfastItemRequest request)
        .ShouldReturnSuccessfulStatus()
        .ShouldReturnJson<BreakfastItem>(r => r.Id.Should().Be(1),
                                         r => r.Name.Should().Be(request.Name),
                                         r => r.Rating.Should().Be(request.Rating))
        .ShouldExistInDatabase<BreakfastItem>(1,
                                        x => x.Id.Should().Be(1),
                                        x => existing.Name.Should().Be(request.Name),
                                        x => x.Rating.Should().Be(request.Rating))
        .RunAsync();
```