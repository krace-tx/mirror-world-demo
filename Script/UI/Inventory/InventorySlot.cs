using Godot;

namespace MirrorWorldDemo.Script.UI.Inventory;

public class InventorySlot
{
	
	// 槽位中的物品模板
	public ItemData Item { get; set; }
	
	// 物品数量
	public int Quantity { get; set; }
	
	// 槽位是否为空
	public bool IsEmpty => Item == null || Quantity <= 0;
	
	// 槽位是否已满
	public bool IsFull => Item != null && Quantity >= Item.MaxStackSize;
	
	// 槽位中还能放入多少物品
	public int RemainingSpace => Item == null
		? int.MaxValue  
		: Item.MaxStackSize - Quantity;
	
	// 清空槽位
	public void Clear()
	{
		Item = null;
		Quantity = 0;
	}
	
	// 创建带物品的槽位
	public static InventorySlot Create(ItemData item, int quantity)
	{
		return new InventorySlot
		{
			Item = item,
			Quantity = Mathf.Min(quantity, item.MaxStackSize)
		};
	}
}
