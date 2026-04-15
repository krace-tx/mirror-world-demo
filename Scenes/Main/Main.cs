using Godot;
using MirrorWorldDemo.Scenes.Agent.Player;
using MirrorWorldDemo.Scenes.Agent.Skadi;
using MirrorWorldDemo.Script.Core;

namespace MirrorWorldDemo.Scenes.Main;

public partial class Main : Node2D
{
	
	private CharacterBody2D  _player;

	private Node2D _farmHouse;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetNodeOrNull<CharacterBody2D>("Player");
		_farmHouse = GetNodeOrNull<Node2D>("FarmHouse/Level_0");
		
		YSortUtil.EnableYSortPath(_player, _farmHouse);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
