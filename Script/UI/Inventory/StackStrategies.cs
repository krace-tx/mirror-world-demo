using System;
namespace MirrorWorldDemo.Script.UI.Inventory;

public interface IStackStrategy
{
	bool CanStack(ItemInstance current, ItemInstance incoming);
	int GetMaxStackSize(ItemInstance item);
}

public class DefaultStackStrategy : IStackStrategy
{
	public bool CanStack(ItemInstance current, ItemInstance incoming)
	{
		if (current == null || incoming == null)
		{
			return false;
		}

		if (GetMaxStackSize(current) <= 1 || GetMaxStackSize(incoming) <= 1)
		{
			return false;
		}

		return current.IsSameStackKey(incoming);
	}

	public int GetMaxStackSize(ItemInstance item)
	{
		return item?.Prototype == null ? 0 : Math.Max(1, item.Prototype.MaxStackSize);
	}
}

public class ShopStackStrategy : IStackStrategy
{
	private readonly bool _allowStacking;

	public ShopStackStrategy(bool allowStacking = false)
	{
		_allowStacking = allowStacking;
	}

	public bool CanStack(ItemInstance current, ItemInstance incoming)
	{
		if (!_allowStacking)
		{
			return false;
		}

		return new DefaultStackStrategy().CanStack(current, incoming);
	}

	public int GetMaxStackSize(ItemInstance item)
	{
		return item?.Prototype == null ? 0 : Math.Max(1, item.Prototype.MaxStackSize);
	}
}
