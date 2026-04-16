using Godot;
using System;
using System.Linq;

namespace MirrorWorldDemo.Script.UI.Inventory;

public class InventoryData
{
	// 背包槽位数组
	private InventorySlot[] _slots;

	// 背包容量
	public int Capacity => _slots.Length;

	/// <summary>
	/// 当前物品总数
	/// </summary>
	public int TotalItemCount => _slots.Sum(s => s.Quantity);

	/// <summary>
	/// 空槽位数量
	/// </summary>
	public int EmptySlotCount => _slots.Count(s => s.IsEmpty);

	/// <summary>
	/// 是否已满
	/// </summary>
	public bool IsFull => EmptySlotCount == 0;

	public InventoryData(int capacity = 36)
	{
		_slots = new InventorySlot[capacity];
	}

	/// <summary>
	/// 添加物品到背包
	/// 核心逻辑：
	/// 1. 先尝试在现有槽位中堆叠（相同物品且未满）
	/// 2. 如果还有剩余，创建新槽位
	/// 3. 如果背包满了，返回未能放入的数量
	/// </summary>
	/// <param name="item">物品模板</param>
	/// <param name="quantity">数量</param>
	/// <returns>未能放入背包的数量（0 表示全部成功）</returns>
	public int AddItem(ItemData item, int quantity)
	{
		if (item == null || quantity <= 0)
			return quantity;

		int remaining = quantity;

		// 第一步：尝试在现有槽位中堆叠
		foreach (ref InventorySlot slot in _slots.AsSpan())
		{
			if (remaining <= 0) break;

			// 检查是否可以堆叠：物品ID相同且槽位未满
			if (slot.Item?.Id == item.Id && !slot.IsFull)
			{
				int canAdd = Mathf.Min(slot.RemainingSpace, remaining);
				slot.Quantity += canAdd;
				remaining -= canAdd;
			}
		}

		// 第二步：如果还有剩余，尝试创建新槽位
		if (remaining > 0)
		{
			foreach (ref InventorySlot slot in _slots.AsSpan())
			{
				if (remaining <= 0) break;

				if (slot.IsEmpty)
				{
					int canAdd = Mathf.Min(item.MaxStackSize, remaining);
					slot.Item = item;
					slot.Quantity = canAdd;
					remaining -= canAdd;
				}
			}
		}

		return remaining;  // 返回未能放入的数量
	}

	/// <summary>
	/// 从背包移除物品
	/// </summary>
	/// <param name="item">物品模板</param>
	/// <param name="quantity">数量</param>
	/// <returns>实际移除的数量</returns>
	public int RemoveItem(ItemData item, int quantity)
	{
		if (item == null || quantity <= 0)
			return 0;

		int toRemove = quantity;
		int removed = 0;

		// 从各个槽位移除（优先从后面的槽位移除，保持顺序）
		for (int i = _slots.Length - 1; i >= 0; i--)
		{
			if (toRemove <= 0) break;

			ref InventorySlot slot = ref _slots[i];
			if (slot.Item?.Id == item.Id)
			{
				int canRemove = Mathf.Min(slot.Quantity, toRemove);
				slot.Quantity -= canRemove;
				toRemove -= canRemove;
				removed += canRemove;

				// 如果槽位空了，清除引用
				if (slot.Quantity <= 0)
				{
					slot.Clear();
				}
			}
		}

		return removed;
	}

	/// <summary>
	/// 交换两个槽位的内容
	/// </summary>
	public void SwapSlots(int indexA, int indexB)
	{
		if (indexA < 0 || indexA >= Capacity || indexB < 0 || indexB >= Capacity)
			return;

		(_slots[indexA], _slots[indexB]) = (_slots[indexB], _slots[indexA]);
	}

	/// <summary>
	/// 移动物品到指定槽位
	/// </summary>
	public bool MoveToSlot(int fromIndex, int toIndex)
	{
		if (fromIndex < 0 || fromIndex >= Capacity || toIndex < 0 || toIndex >= Capacity)
			return false;

		if (fromIndex == toIndex) return true;

		ref InventorySlot from = ref _slots[fromIndex];
		ref InventorySlot to = ref _slots[toIndex];

		// 目标槽位为空：直接移动
		if (to.IsEmpty)
		{
			to = from;
			from.Clear();
			return true;
		}

		// 目标槽位有物品：
		// 情况1：物品相同，可以堆叠
		if (to.Item?.Id == from.Item?.Id)
		{
			int totalQuantity = to.Quantity + from.Quantity;
			int canStack = Mathf.Min(totalQuantity, to.Item.MaxStackSize);

			to.Quantity = canStack;
			from.Quantity = totalQuantity - canStack;

			// 如果 from 还有剩余，保持原样；否则清空
			if (from.Quantity <= 0)
				from.Clear();

			return true;
		}

		// 情况2：物品不同，交换位置
		SwapSlots(fromIndex, toIndex);
		return true;
	}

	/// <summary>
	/// 获取某个物品的总数
	/// </summary>
	public int GetItemCount(ItemData item)
	{
		if (item == null) return 0;
		return _slots
			.Where(s => s.Item?.Id == item.Id)
			.Sum(s => s.Quantity);
	}

	/// <summary>
	/// 检查是否包含指定物品
	/// </summary>
	public bool HasItem(ItemData item, int quantity = 1)
	{
		return GetItemCount(item) >= quantity;
	}

	/// <summary>
	/// 获取槽位引用（用于UI绑定）
	/// </summary>
	public InventorySlot GetSlot(int index)
	{
		if (index < 0 || index >= Capacity)
			return new InventorySlot();
		return _slots[index];
	}

	/// <summary>
	/// 查找第一个包含指定物品的槽位索引
	/// </summary>
	public int FindSlot(ItemData item)
	{
		for (int i = 0; i < _slots.Length; i++)
		{
			if (_slots[i].Item?.Id == item.Id)
				return i;
		}
		return -1;
	}
}
