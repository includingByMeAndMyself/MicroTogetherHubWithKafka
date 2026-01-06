using Topic.CommandService.Api.Comments.Commands.CreateComment;
using Topic.CommandService.Api.Comments.Commands.RemoveComment;
using Topic.CommandService.Api.Comments.Commands.UpdateComment;

namespace Topic.CommandService.Api.Comments;

public interface ICommentCommandHandler
{
    Task HandleAsync(CreateCommentCommand command, CancellationToken ct);
    Task HandleAsync(UpdateCommentCommand command, CancellationToken ct);
    Task HandleAsync(RemoveCommentCommand command, CancellationToken ct);
}