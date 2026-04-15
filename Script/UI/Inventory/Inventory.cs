using Godot;
using System.Collections.Generic;
namespace MirrorWorldDemo.Script.UI.Inventory;

/// <summary>
/// Inventory UI view/controller.
/// Keeps rendering concerns here and delegates data logic to InventoryData.
/// </summary>
public partial class Intventory : Control
{
	[Export(PropertyHint.Range, "1,12,1")] public int Columns { get; set; } = 4;
	[Export(PropertyHint.Range, "1,12,1")] public int ExtraRows { get; set; } = 6;
	[Export(PropertyHint.Range, "8,12,1")] public int HotbarCount { get; set; } = 12;
	[Export] public Vector2 SlotSize { get; set; } = new(44, 44);
	[Export] public bool ShowDebugText { get; set; } = true;
	[Export] public Godot.Collections.Array<ItemPrototype> DebugStartingItems { get; set; } = new();

	private HBoxContainer _hotbarRow;
	private GridContainer _backpackGrid;
	private PanelContainer _framePanel;
	private readonly Dictionary<int, SlotWidgets> _slotWidgets = new();
	private Inventory _inventory;

	public override void _Ready()
	{
		_hotbarRow = GetNode<HBoxContainer>("InventoryUI/FramePanel/Padding/RootVBox/ContentRow/SlotColumn/HotbarRow");
		_backpackGrid = GetNode<GridContainer>("InventoryUI/FramePanel/Padding/RootVBox/ContentRow/SlotColumn/BackpackGrid");
		_framePanel = GetNode<PanelContainer>("InventoryUI/FramePanel");

		ApplyFrameStyle();
		RebuildSlotWidgets();
		BuildInventoryData();
		SeedDebugItems();
		RefreshAllSlots();
	}

	private void BuildInventoryData()
	{
		_inventory = new Inventory(HotbarCount + (Columns * ExtraRows), InventoryOperationType.PlayerInventory, new DefaultStackStrategy());
		_inventory.OnSlotChanged += RefreshSlot;
	}

	private void SeedDebugItems()
	{
		for (int i = 0; i < DebugStartingItems.Count; i++)
		{
			ItemPrototype prototype = DebugStartingItems[i];
			if (prototype == null)
			{
				continue;
			}

			_inventory.AddItem(new ItemInstance(prototype), 1);
		}
	}

	private void ApplyFrameStyle()
	{
		StyleBoxFlat frame = new()
		{
			BgColor = new Color("b88b5f"),
			BorderColor = new Color("4b2f1f"),
			BorderWidthLeft = 4,
			BorderWidthTop = 4,
			BorderWidthRight = 4,
			BorderWidthBottom = 4,
			CornerRadiusTopLeft = 8,
			CornerRadiusTopRight = 8,
			CornerRadiusBottomRight = 8,
			CornerRadiusBottomLeft = 8
		};
		_framePanel.AddThemeStyleboxOverride("panel", frame);
	}

	private void RebuildSlotWidgets()
	{
		_backpackGrid.Columns = Columns;
		_slotWidgets.Clear();

		foreach (Node child in _hotbarRow.GetChildren())
		{
			child.QueueFree();
		}

		foreach (Node child in _backpackGrid.GetChildren())
		{
			child.QueueFree();
		}

		for (int i = 0; i < HotbarCount; i++)
		{
			Control slotNode = CreateSlotNode(i, selected: i == 0);
			_hotbarRow.AddChild(slotNode);
		}

		int totalBackpackSlots = Columns * ExtraRows;
		for (int i = 0; i < totalBackpackSlots; i++)
		{
			int slotIndex = HotbarCount + i;
			Control slotNode = CreateSlotNode(slotIndex, selected: false);
			_backpackGrid.AddChild(slotNode);
		}
	}

	private Control CreateSlotNode(int slotIndex, bool selected)
	{
		PanelContainer slotPanel = new();
		slotPanel.CustomMinimumSize = SlotSize;
		slotPanel.MouseFilter = MouseFilterEnum.Stop;

		StyleBoxFlat slotStyle = new()
		{
			BgColor = selected ? new Color("f6d98a") : new Color("ead8b2"),
			BorderColor = selected ? new Color("ad6d2f") : new Color("8f6b45"),
			BorderWidthLeft = 2,
			BorderWidthTop = 2,
			BorderWidthRight = 2,
			BorderWidthBottom = 2,
			CornerRadiusTopLeft = 6,
			CornerRadiusTopRight = 6,
			CornerRadiusBottomRight = 6,
			CornerRadiusBottomLeft = 6
		};
		slotPanel.AddThemeStyleboxOverride("panel", slotStyle);

		MarginContainer margin = new();
		margin.AddThemeConstantOverride("margin_left", 6);
		margin.AddThemeConstantOverride("margin_top", 6);
		margin.AddThemeConstantOverride("margin_right", 6);
		margin.AddThemeConstantOverride("margin_bottom", 6);
		slotPanel.AddChild(margin);

		VBoxContainer root = new();
		margin.AddChild(root);

		Label iconText = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill,
			Text = string.Empty
		};
		root.AddChild(iconText);

		Label quantityText = new()
		{
			HorizontalAlignment = HorizontalAlignment.Right,
			Text = string.Empty
		};
		root.AddChild(quantityText);

		_slotWidgets[slotIndex] = new SlotWidgets(iconText, quantityText);
		return slotPanel;
	}

	private void RefreshAllSlots()
	{
		foreach (int slotIndex in _slotWidgets.Keys)
		{
			RefreshSlot(slotIndex);
		}
	}

	private void RefreshSlot(int slotIndex)
	{
		if (_inventory == null || !_slotWidgets.TryGetValue(slotIndex, out SlotWidgets widgets))
		{
			return;
		}

		Slot slot = _inventory.GetSlot(slotIndex);
		if (slot.IsEmpty)
		{
			widgets.IconLabel.Text = ShowDebugText ? "-" : string.Empty;
			widgets.QuantityLabel.Text = string.Empty;
			return;
		}

		if (ShowDebugText)
		{
			string id = slot.Item.QualifiedItemId;
			widgets.IconLabel.Text = id.Length > 0 ? id[..1].ToUpperInvariant() : "?";
		}
		else
		{
			widgets.IconLabel.Text = string.Empty;
		}

		widgets.QuantityLabel.Text = slot.Amount > 1 ? slot.Amount.ToString() : string.Empty;
	}

	private readonly struct SlotWidgets
	{
		public Label IconLabel { get; }
		public Label QuantityLabel { get; }

		public SlotWidgets(Label iconLabel, Label quantityLabel)
		{
			IconLabel = iconLabel;
			QuantityLabel = quantityLabel;
		}
	}
}
