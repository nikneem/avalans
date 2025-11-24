namespace Bexter.Avalans.Core;

/// <summary>
/// Handler for commands that return a result
/// </summary>
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// Handler for commands with no return value (fire-and-forget)
/// </summary>
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task Handle(TCommand command, CancellationToken cancellationToken);
}
