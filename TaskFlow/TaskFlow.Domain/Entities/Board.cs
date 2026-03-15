namespace TaskFlow.Domain.Entities;

public class Board
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User Owner { get; set; } = null!;
    public ICollection<Column> Columns { get; set; } = [];
    public ICollection<BoardMember> Members { get; set; } = [];
}