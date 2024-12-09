using System.Diagnostics.CodeAnalysis;

namespace Mvp.Selections.Domain;

[SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Match the names of the networks.")]
public enum ProfileLinkType
{
    Other,
    Blog,
    StackExchange,
    Community,
    Twitter,
    Youtube,
    Github,
    LinkedIn,
    Slack,
    Bluesky
}