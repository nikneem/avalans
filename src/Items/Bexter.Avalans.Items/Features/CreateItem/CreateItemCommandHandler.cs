using Bexter.Avalans.Core;
using Bexter.Avalans.Items.Domain;
using Bexter.Avalans.Items.Repositories;

namespace Bexter.Avalans.Items.Features.CreateItem;

/// <summary>
/// Handler for CreateItemCommand.
/// Orchestrates item creation through the domain model.
/// </summary>
public sealed class CreateItemCommandHandler : ICommandHandler<CreateItemCommand, CreateItemResult>
{
    private readonly IItemRepository _repository;

    public CreateItemCommandHandler(IItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateItemResult> Handle(CreateItemCommand command, CancellationToken cancellationToken)
    {
        // Create item through domain model factory method (validates business rules)
        var item = Item.Create(command.Name, command.Description);

        // Set optional properties through domain methods (each validates invariants)
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

        if (command.Value.HasValue)
        {
            item.SetValue(command.Value.Value);
        }

        // Persist through repository
        var itemId = await _repository.AddAsync(item, cancellationToken);

        return new CreateItemResult(itemId);
    }
}
