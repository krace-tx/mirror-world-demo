using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
namespace MirrorWorldDemo.Script.UI.Inventory;

public class Inventory : IEnumerable<Slot>
{
	public event Action<int> OnSlotChanged;
	public event Action<ItemInstance, int> OnItemAdded;
	public event Action<ItemInstance, int> OnItemRemoved;

	public Predicate<ItemInstance> CanAcceptItem { get; set; } = static _ => true;
	public Predicate<ItemInstance> CanRemoveItem { get; set; } = static _ => true;
	public Predicate<int> CanSwapFromSlot { get; set; } = static _ => true;

	public InventoryOperationType OperationType { get; }
	public int Capacity => _slots.Count;

	private readonly List<Slot> _slots;

	public Inventory(int capacity, InventoryOperationType operationType = InventoryOperationType.PlayerInventory, IStackStrategy stackStrategy = null)
	{
		if (capacity <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(capacity), "Inventory capacity must be greater than 0.");
		}

		OperationType = operationType;
		_slots = new List<Slot>(capacity);
		for (int i = 0; i < capacity; i++)
		{
			Slot slot = new Slot(i, this, stackStrategy);
			slot.OnChanged += HandleSlotChanged;
			_slots.Add(slot);
		}
	}

	public Slot this[int index] => _slots[index];

	public Slot GetSlot(int index)
	{
		if (!IsIndexValid(index))
		{
			throw new ArgumentOutOfRangeException(nameof(index));
		}

		return _slots[index];
	}

	public bool TryGetSlot(int index, out Slot slot)
	{
		if (!IsIndexValid(index))
		{
			slot = null;
			return false;
		}

		slot = _slots[index];
		return true;
	}

	public int AddItem(ItemInstance item, int amount)
	{
		if (!CanAccept(item, amount))
		{
			return 0;
		}

		int remaining = amount;
		int totalAdded = 0;

		for (int i = 0; i < _slots.Count && remaining > 0; i++)
		{
			Slot slot = _slots[i];
			if (slot.IsEmpty)
			{
				continue;
			}

			int added = slot.AddItem(item, remaining);
			if (added <= 0)
			{
				continue;
			}

			remaining -= added;
			totalAdded += added;
		}

		for (int i = 0; i < _slots.Count && remaining > 0; i++)
		{
			Slot slot = _slots[i];
			if (!slot.IsEmpty)
			{
				continue;
			}

			int added = slot.AddItem(item, remaining);
			if (added <= 0)
			{
				continue;
			}

			remaining -= added;
			totalAdded += added;
		}

		if (totalAdded > 0)
		{
			OnItemAdded?.Invoke(item.Clone(), totalAdded);
		}

		return totalAdded;
	}

	public int RemoveItem(ItemInstance item, int amount)
	{
		if (!CanRemove(item, amount))
		{
			return 0;
		}

		int remaining = amount;
		int totalRemoved = 0;
		for (int i = 0; i < _slots.Count && remaining > 0; i++)
		{
			Slot slot = _slots[i];
			if (slot.IsEmpty || slot.Item.QualifiedItemId != item.QualifiedItemId)
			{
				continue;
			}

			int removed = slot.RemoveItem(remaining);
			if (removed <= 0)
			{
				continue;
			}

			remaining -= removed;
			totalRemoved += removed;
		}

		if (totalRemoved > 0)
		{
			OnItemRemoved?.Invoke(item.Clone(), totalRemoved);
		}

		return totalRemoved;
	}

	public int RemoveFromSlot(int slotIndex, int amount)
	{
		if (!TryGetSlot(slotIndex, out Slot slot) || slot.IsEmpty || amount <= 0 || !CanRemove(slot.Item, amount))
		{
			return 0;
		}

		ItemInstance removedItem = slot.Item.Clone();
		int removed = slot.RemoveItem(amount);
		if (removed > 0)
		{
			OnItemRemoved?.Invoke(removedItem, removed);
		}

		return removed;
	}

	public bool MoveSlot(int fromIndex, int toIndex)
	{
		if (!IsIndexValid(fromIndex) || !IsIndexValid(toIndex) || fromIndex == toIndex)
		{
			return false;
		}

		if (!CanSwapFromSlot(fromIndex) || !CanSwapFromSlot(toIndex))
		{
			return false;
		}

		Slot from = _slots[fromIndex];
		Slot to = _slots[toIndex];
		if (from.IsEmpty)
		{
			return false;
		}

		int moved = to.AddItem(from.Item, from.Amount);
		if (moved > 0)
		{
			from.RemoveItem(moved);
			if (from.IsEmpty || to.IsEmpty)
			{
				return true;
			}
		}

		from.Swap(to);
		return true;
	}

	public bool SwapSlots(int indexA, int indexB)
	{
		if (!IsIndexValid(indexA) || !IsIndexValid(indexB) || indexA == indexB)
		{
			return false;
		}

		if (!CanSwapFromSlot(indexA) || !CanSwapFromSlot(indexB))
		{
			return false;
		}

		_slots[indexA].Swap(_slots[indexB]);
		return true;
	}

	public bool HasSpaceFor(ItemInstance item, int amount)
	{
		if (!CanAccept(item, amount))
		{
			return false;
		}

		int remaining = amount;
		for (int i = 0; i < _slots.Count; i++)
		{
			Slot slot = _slots[i];
			int free = slot.GetRemainingStackCapacity(item);
			if (free <= 0)
			{
				continue;
			}

			remaining -= free;
			if (remaining <= 0)
			{
				return true;
			}
		}

		return false;
	}

	public int GetItemTotalCount(string qualifiedItemId)
	{
		if (string.IsNullOrWhiteSpace(qualifiedItemId))
		{
			return 0;
		}

		int total = 0;
		for (int i = 0; i < _slots.Count; i++)
		{
			Slot slot = _slots[i];
			if (slot.IsEmpty)
			{
				continue;
			}

			if (slot.Item.QualifiedItemId == qualifiedItemId)
			{
				total += slot.Amount;
			}
		}

		return total;
	}

	public void Clear()
	{
		for (int i = 0; i < _slots.Count; i++)
		{
			_slots[i].Clear();
		}
	}

	public Godot.Collections.Array<Godot.Collections.Dictionary> BuildSaveData()
	{
		Godot.Collections.Array<Godot.Collections.Dictionary> saveData = new();
		for (int i = 0; i < _slots.Count; i++)
		{
			Slot slot = _slots[i];
			if (slot.IsEmpty)
			{
				saveData.Add(new Godot.Collections.Dictionary());
				continue;
			}

			saveData.Add(new Godot.Collections.Dictionary
			{
				["itemId"] = slot.Item.QualifiedItemId,
				["qty"] = slot.Amount,
				["customData"] = slot.Item.CustomData
			});
		}

		return saveData;
	}

	public void LoadSaveData(Godot.Collections.Array<Godot.Collections.Dictionary> saveData, Func<string, Script.UI.Inventory.ItemPrototype> prototypeResolver)
	{
		if (saveData == null || prototypeResolver == null)
		{
			return;
		}

		Clear();
		int count = Math.Min(saveData.Count, _slots.Count);
		for (int i = 0; i < count; i++)
		{
			var row = saveData[i];
			if (row == null || row.Count == 0 || !row.ContainsKey("itemId"))
			{
				continue;
			}

			string itemId = row["itemId"].AsString();
			int qty = row.ContainsKey("qty") ? row["qty"].AsInt32() : 0;
			string customData = row.ContainsKey("customData") ? row["customData"].AsString() : string.Empty;
			if (qty <= 0 || string.IsNullOrWhiteSpace(itemId))
			{
				continue;
			}

			Script.UI.Inventory.ItemPrototype prototype = prototypeResolver(itemId);
			if (prototype == null)
			{
				continue;
			}

			// Durability has been removed from ItemInstance. Keep loading old saves by ignoring that field.
			_slots[i].SetContents(new ItemInstance(prototype, customData), qty);
		}
	}

	public IEnumerator<Slot> GetEnumerator()
	{
		return _slots.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private bool IsIndexValid(int index)
	{
		return index >= 0 && index < _slots.Count;
	}

	private bool CanAccept(ItemInstance item, int amount)
	{
		return item != null && item.IsValid && amount > 0 && CanAcceptItem(item);
	}

	private bool CanRemove(ItemInstance item, int amount)
	{
		return item != null && item.IsValid && amount > 0 && CanRemoveItem(item);
	}

	private void HandleSlotChanged(Slot slot)
	{
		OnSlotChanged?.Invoke(slot.SlotId);
	}
}
