using FluentAssertions;
using UnoFramework.Contracts.Busy;
using NUnit.Framework;

namespace UnoFramework.Tests;

[TestFixture]
public class GlobalBusyEventTests
{
    [Test]
    public void GlobalBusyEvent_ShouldHaveCorrectProperties()
    {
        var evt = new GlobalBusyEvent(true, "Loading...");

        evt.IsBusy.Should().BeTrue();
        evt.Message.Should().Be("Loading...");
    }

    [Test]
    public void GlobalBusyEvent_MessageCanBeNull()
    {
        var evt = new GlobalBusyEvent(false);

        evt.IsBusy.Should().BeFalse();
        evt.Message.Should().BeNull();
    }
}
