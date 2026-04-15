using Godot;
using MirrorWorldDemo.Script.Core;

namespace MirrorWorldDemo.Scenes.Room;

public partial class Room : Node2D
{
	private TileMapLayer _frontTable;
	private TileMapLayer _buildingTable;
	private TileMapLayer _buildingWalls;
	private TileMapLayer _backFloors;
	private TileMapLayer _buildingDecorate;
	private Node2D _light;

	public override void _Ready()
	{
		_frontTable = GetNodeOrNull<TileMapLayer>("Level_0/FrontTable");

		_buildingTable = GetNodeOrNull<TileMapLayer>("Level_0/BuildingTable");
		_buildingWalls = GetNodeOrNull<TileMapLayer>("Level_0/BuildingWalls");
		_buildingDecorate = GetNodeOrNull<TileMapLayer>("Level_0/BuildingDecorate");

		_backFloors = GetNodeOrNull<TileMapLayer>("Level_0/BackFloors");

		_light = GetNodeOrNull<Node2D>("Level_0/Light");
			
		TileMapLayerUtil.SetAsFrontLayer(_frontTable);
		
		TileMapLayerUtil.SetAsBuildingLayer(_buildingWalls, CollisionLayerEnum.Wall);
		TileMapLayerUtil.SetAsBuildingLayer(_buildingTable, CollisionLayerEnum.Object);
		TileMapLayerUtil.SetAsBuildingLayer(_buildingDecorate, CollisionLayerEnum.Object);
		
		TileMapLayerUtil.SetAsBackLayer(_backFloors);

	}

	public override void _Process(double delta)
	{
	}
}
