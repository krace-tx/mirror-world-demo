using System;
namespace MirrorWorldDemo.Script.UI.Inventory;

/// <summary>
/// Smallest mutable unit in an inventory.
/// Handles stack operations and emits change notifications.
/// </summary>
public class Slot
{
	public event Action<Slot> OnChanged;

	public int SlotId { get; }
	public Inventory Owner { get; }
	public ItemInstance Item { get; private set; }
	public int Amount { get; private set; }
	public bool IsEmpty => Item == null || Amount <= 0;

	private readonly IStackStrategy _stackStrategy;

	public Slot(int slotId, Inventory owner, IStackStrategy stackStrategy = null)
	{
		SlotId = slotId;
		Owner = owner;
		_stackStrategy = stackStrategy ?? new DefaultStackStrategy();
	}

	public bool CanAddItem(ItemInstance item, int amount)
	{
		if (item == null || !item.IsValid || amount <= 0)
		{
			return false;
		}

		return GetRemainingStackCapacity(item) >= amount;
	}

	public int AddItem(ItemInstance item, int amount)
	{
		if (item == null || !item.IsValid || amount <= 0)
		{
			return 0;
		}

		int addable = Math.Min(amount, GetRemainingStackCapacity(item));
		if (addable <= 0)
		{
			return 0;
		}

		if (IsEmpty)
		{
			// Store a clone to prevent outside references from mutating slot state.
			Item = item.Clone();
			Amount = addable;
		}
		else
		{
			Amount += addable;
		}

		OnChanged?.Invoke(this);
		return addable;
	}

	public int RemoveItem(int amount)
	{
		if (IsEmpty || amount <= 0)
		{
			return 0;
		}

		int removed = Math.Min(Amount, amount);
		Amount -= removed;
		if (Amount <= 0)
		{
			// Normalize empty state.
			Item = null;
			Amount = 0;
		}

		OnChanged?.Invoke(this);
		return removed;
	}

	public void Swap(Slot other)
	{
		if (other == null || ReferenceEquals(other, this))
		{
			return;
		}

		(Item, other.Item) = (other.Item, Item);
		(Amount, other.Amount) = (other.Amount, Amount);

		OnChanged?.Invoke(this);
		other.OnChanged?.Invoke(other);
	}

	public void Clear()
	{
		if (IsEmpty)
		{
			return;
		}

		Item = null;
		Amount = 0;
		OnChanged?.Invoke(this);
	}

	internal void SetContents(ItemInstance item, int amount)
	{
		if (item == null || amount <= 0)
		{
			Item = null;
			Amount = 0;
			OnChanged?.Invoke(this);
			return;
		}

		Item = item.Clone();
		Amount = amount;
		OnChanged?.Invoke(this);
	}

	internal int GetRemainingStackCapacity(ItemInstance incoming)
	{
		if (incoming == null || !incoming.IsValid)
		{
			return 0;
		}

		if (IsEmpty)
		{
			// Empty slot can accept up to max stack size for this item.
			return _stackStrategy.GetMaxStackSize(incoming);
		}

		if (!_stackStrategy.CanStack(Item, incoming))
		{
			return 0;
		}

		return Math.Max(0, _stackStrategy.GetMaxStackSize(Item) - Amount);
	}
}
