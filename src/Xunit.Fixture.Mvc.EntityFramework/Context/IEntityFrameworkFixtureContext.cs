using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Xunit.Fixture.Mvc.EntityFramework.Context
{
    internal interface IEntityFrameworkFixtureContext
    {
        Task BootstrapAsync();

        DbContext Context { get; }
    }
}