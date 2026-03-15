using TaskFlow.Domain.Enum;

namespace TaskFlow.Domain.Entities;

public class BoardMember
{
    public Guid BoardId { get; set; }
    public Guid UserId { get; set; }
    public BoardMemberRole Role { get; set; } = BoardMemberRole.Member; // Owner, Member, Viewer

    public Board Board { get; set; } = null!;
    public User User { get; set; } = null!;
}
