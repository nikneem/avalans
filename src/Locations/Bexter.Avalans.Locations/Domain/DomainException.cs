namespace Bexter.Avalans.Locations.Domain;

/// <summary>
/// Custom exception for domain rule violations in the Locations domain.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
