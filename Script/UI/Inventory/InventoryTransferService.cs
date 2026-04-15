using System;
namespace MirrorWorldDemo.Script.UI.Inventory;

/// <summary>
/// Moves items from one inventory to another.
/// </summary>
public static class InventoryTransferService
{
	/// <summary>
	/// amount < 0 means "move all".
	/// </summary>
	public static int Transfer(Inventory source, int sourceSlotIndex, Inventory target, int amount)
	{
		if (source == null || target == null || amount == 0)
		{
			return 0;
		}

		if (!source.TryGetSlot(sourceSlotIndex, out Slot sourceSlot) || sourceSlot.IsEmpty)
		{
			return 0;
		}

		int desiredAmount = amount < 0 ? sourceSlot.Amount : Math.Min(amount, sourceSlot.Amount);
		if (desiredAmount <= 0)
		{
			return 0;
		}

		ItemInstance movingItem = sourceSlot.Item.Clone();
		// Respect container-level rules configured by caller.
		if (!target.CanAcceptItem(movingItem) || !source.CanRemoveItem(movingItem))
		{
			return 0;
		}

		int added = target.AddItem(movingItem, desiredAmount);
		if (added <= 0)
		{
			return 0;
		}

		int removed = source.RemoveFromSlot(sourceSlotIndex, added);
		if (removed != added)
		{
			throw new InvalidOperationException("Inventory transfer moved mismatched amounts.");
		}

		return removed;
	}
}
