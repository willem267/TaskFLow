namespace TaskFlow.Application.DTOs.Realtime;

public record TaskMovedPayload(
    Guid TaskId,
    Guid TargetColumnId,
    float NewPosition
);