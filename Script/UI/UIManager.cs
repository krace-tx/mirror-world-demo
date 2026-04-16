using Godot;

namespace MirrorWorldDemo.Script.UI;

/// <summary>
/// Central UI assembler/controller.
/// Owns screen-level layering and toggles feature panels like Inventory.
/// </summary>
public partial class UIManager : CanvasLayer
{
	[Export]
	public string InventoryScenePath { get; set; } = "res://Scenes/UI/Inventory/InventoryView.tscn";

	[Export]
	public StringName ToggleInventoryAction { get; set; } = "ui_inventory";

	private Control _inventory;
	private ColorRect _dimmer;
	private Control _mountPoint;

	public override void _Ready()
	{
		_dimmer = GetNodeOrNull<ColorRect>("OverlayRoot/Dimmer");
		_mountPoint = GetNodeOrNull<Control>("OverlayRoot/CenterMount");

		InstantiateInventory();
		SetInventoryVisible(false);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (IsInventoryToggleInput(@event))
		{
			ToggleInventory();
			GetViewport().SetInputAsHandled();
		}
	}

	public void ToggleInventory()
	{
		if (_inventory == null)
		{
			InstantiateInventory();
		}

		if (_inventory == null)
		{
			return;
		}

		SetInventoryVisible(!_inventory.Visible);
	}

	public void SetInventoryVisible(bool visible)
	{
		if (_inventory != null)
		{
			_inventory.Visible = visible;
		}

		if (_dimmer != null)
		{
			_dimmer.Visible = visible;
		}
	}

	private void InstantiateInventory()
	{
		if (_inventory != null || _mountPoint == null)
		{
			return;
		}

		if (string.IsNullOrWhiteSpace(InventoryScenePath))
		{
			GD.PushWarning("UIManager: InventoryScenePath is empty.");
			return;
		}

		PackedScene inventoryScene = GD.Load<PackedScene>(InventoryScenePath);
		if (inventoryScene == null)
		{
			GD.PushWarning($"UIManager: Failed to load Inventory scene at '{InventoryScenePath}'.");
			return;
		}

		_inventory = inventoryScene.Instantiate<Control>();
		_inventory.Name = "Inventory";
		_mountPoint.AddChild(_inventory);
	}

	private bool IsInventoryToggleInput(InputEvent @event)
	{
		if (InputMap.HasAction(ToggleInventoryAction) && @event.IsActionPressed(ToggleInventoryAction))
		{
			return true;
		}

		if (@event is InputEventKey keyEvent &&
			keyEvent.Pressed &&
			!keyEvent.Echo &&
			keyEvent.Keycode == Key.I)
		{
			return true;
		}

		return false;
	}
}
