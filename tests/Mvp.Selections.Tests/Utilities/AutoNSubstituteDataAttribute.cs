using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace Mvp.Selections.Tests.Utilities
{
    public class AutoNSubstituteDataAttribute() : AutoDataAttribute(() =>
    {
        IFixture? fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        return fixture;
    });
}
