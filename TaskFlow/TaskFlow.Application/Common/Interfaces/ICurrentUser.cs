namespace TaskFlow.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid Id { get; }
    string Email { get; }
    string Name { get; }
}