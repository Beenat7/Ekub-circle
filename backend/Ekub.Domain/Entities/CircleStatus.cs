namespace Ekub.Domain.Entities;

public enum CircleStatus
{
    Forming = 0,
    NotStarted = 0, // Alias for legacy database records/compatibility
    Active = 1,
    Completed = 2
}