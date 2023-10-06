using Impostor.Api.Innersloth;
using Xunit;

namespace Impostor.Tests;

public sealed class GameVersionTests
{
    [Theory]
    [InlineData(50588150, 2023, 7, 11)]
    [InlineData(50588175, 2023, 7, 11, 25)]
    public void Test(int value, int year, int month, int day, int revision = 0)
    {
        var gameVersion = new GameVersion(year, month, day, revision);

        Assert.Equal(value, gameVersion.Value);

        gameVersion.GetComponents(out var parsedYear, out var parsedMonth, out var parsedDay, out var parsedRevision);
        Assert.Equal(year, parsedYear);
        Assert.Equal(month, parsedMonth);
        Assert.Equal(day, parsedDay);
        Assert.Equal(revision, parsedRevision);

        Assert.Equal(year, gameVersion.Year);
        Assert.Equal(month, gameVersion.Month);
        Assert.Equal(day, gameVersion.Day);
        Assert.Equal(revision, gameVersion.Revision);
    }
}
