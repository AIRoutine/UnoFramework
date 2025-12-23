using FluentAssertions;
using UnoFramework.Contracts.Busy;
using NUnit.Framework;

namespace UnoFramework.Tests;

[TestFixture]
public class BusyModeTests
{
    [Test]
    public void BusyMode_None_ShouldBeZero()
    {
        ((int)BusyMode.None).Should().Be(0);
    }

    [Test]
    public void BusyMode_Local_ShouldBeOne()
    {
        ((int)BusyMode.Local).Should().Be(1);
    }

    [Test]
    public void BusyMode_Global_ShouldBeTwo()
    {
        ((int)BusyMode.Global).Should().Be(2);
    }
}
