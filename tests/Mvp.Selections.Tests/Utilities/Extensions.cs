using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Mvp.Selections.Tests.Utilities;

public static class Extensions
{
    public static void ReceivedAndContains<T>(this ILogger<T> logger, LogLevel level, string message)
    {
        logger.Received(1).Log(
            level,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains(message)),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }
}