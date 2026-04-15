using Godot;
using MirrorWorldDemo.Script.Core;

namespace MirrorWorldDemo.Scenes.Farm;

public partial class FarmHouse : Node2D
{
	private TileMapLayer _frontWalls;
	private TileMapLayer _buildingWalls;
	private TileMapLayer _backFloors;
	private TileMapLayer _backWalls;      
	private TileMapLayer _backWalls1;
	private Node2D _buildingEntity;

	public override void _Ready()
	{
		_frontWalls = GetNode<TileMapLayer>("Level_0/FrontWalls");
		
		_buildingWalls = GetNodeOrNull<TileMapLayer>("Level_0/BuildingWalls");
		
		_backFloors = GetNodeOrNull<TileMapLayer>("Level_0/BackFloors");
		_backWalls = GetNodeOrNull<TileMapLayer>("Level_0/BackWalls");
		_backWalls1 = GetNodeOrNull<TileMapLayer>("Level_0/BackWalls/BackWalls1");
		
		_buildingEntity = GetNodeOrNull<Node2D>("Level_0/BuildingEntity");

		TileMapLayerUtil.SetAsFrontLayer(_frontWalls);
		
		TileMapLayerUtil.SetAsBuildingLayer(_buildingWalls, CollisionLayerEnum.Wall);
		
		TileMapLayerUtil.SetAsBackLayer(_backFloors);
		TileMapLayerUtil.SetAsBackLayer(_backWalls);
		TileMapLayerUtil.SetAsBackLayer(_backWalls1);
		
	}
}
