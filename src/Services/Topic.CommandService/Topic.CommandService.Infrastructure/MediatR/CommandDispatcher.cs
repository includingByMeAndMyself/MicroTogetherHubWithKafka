using Core.Commands;
using Core.MediatR;

namespace Topic.CommandService.Infrastructure.MediatR;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();

    public void RegisterHandler<T>(Func<T, Task> handler)
        where T : BaseCommand
    {
        if (_handlers.ContainsKey(typeof(T)))
        {
            throw new InvalidOperationException(
                "Попытка повторной регистрации обработчика команд"
            );
        }
        _handlers.Add(typeof(T), item => handler((T)item));
    }

    public async Task SendCommandAsync(BaseCommand command)
    {
        if (_handlers.TryGetValue(command.GetType(),
                out Func<BaseCommand, Task>? handler))
        {
            await handler(command);
        }
        else
        {
            throw new InvalidOperationException(
                "Обработчик команд не был зарегистрирован"
            );
        }
    }
}