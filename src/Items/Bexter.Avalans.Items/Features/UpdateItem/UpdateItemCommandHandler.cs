using Bexter.Avalans.Core;
using Bexter.Avalans.Items.Domain;
using Bexter.Avalans.Items.Repositories;

namespace Bexter.Avalans.Items.Features.UpdateItem;

/// <summary>
/// Handler for UpdateItemCommand.
/// Orchestrates item updates through the domain model.
/// </summary>
public sealed class UpdateItemCommandHandler : ICommandHandler<UpdateItemCommand>
{
    private readonly IItemRepository _repository;

    public UpdateItemCommandHandler(IItemRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateItemCommand command, CancellationToken cancellationToken)
    {
        // Load aggregate root
        var item = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (item is null)
        {
            throw new DomainException($"Item with ID {command.Id} not found");
        }

        // Update through domain methods (validates business rules)
        item.UpdateBasicInfo(command.Name, command.Description);

        if (command.Length.HasValue && command.Width.HasValue && command.Height.HasValue)
        {
            item.SetDimensions(command.Length, command.Width, command.Height);
        }

        if (command.Weight.HasValue)
        {
            item.SetWeight(command.Weight.Value);
        }

        if (command.IsPerishable)
        {
            if (!command.ShelfLifeDays.HasValue)
            {
                throw new DomainException("Perishable items must have a shelf life specified");
            }
            item.SetPerishability(command.IsPerishable, command.ShelfLifeDays.Value);
        }
        else
        {
            // If not perishable, clear perishability
            item.SetPerishability(false, null);
        }

        if (command.Value.HasValue)
        {
            item.SetValue(command.Value.Value);
        }

        // Persist valid state
        await _repository.UpdateAsync(item, cancellationToken);
    }
}
