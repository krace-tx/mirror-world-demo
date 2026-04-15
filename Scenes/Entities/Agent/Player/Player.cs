using Godot;
using MirrorWorldDemo.Script.Core;

namespace MirrorWorldDemo.Scenes.Agent.Player;

public partial class Player : CharacterBody2D
{
	public const float Speed = 100.0f;
	public const float Friction = 400.0f;

	// ── 状态定义 ────────────────────────────────────────────
	private enum PlayerState
	{
		Idle,
		MoveLeft,
		MoveRight,
		MoveUp,
		MoveDown
	}
	private PlayerState _currentState = PlayerState.Idle;

	private AnimationPlayer _animationPlayer;
	private Sprite2D _body;

	// ── 初始化 ──────────────────────────────────────────────
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_body = GetNode<Sprite2D>("Body");

		this.YSortEnabled = true;
		this.ZIndex = 0;

		this.CollisionLayer = (uint)CollisionLayerEnum.Player;
		this.CollisionMask  = (uint)(CollisionLayerEnum.Wall | CollisionLayerEnum.Object);
	}

	// ── 物理帧 ──────────────────────────────────────────────
	public override void _PhysicsProcess(double delta)
	{
		HandleInput(delta);
		UpdateAnimation();
		MoveAndSlide();
	}

	private void HandleInput(double delta)
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		if (direction != Vector2.Zero)
		{
			Velocity = direction * Speed;
			
			if (Mathf.Abs(direction.X) >= Mathf.Abs(direction.Y))
				_currentState = direction.X < 0 ? PlayerState.MoveLeft : PlayerState.MoveRight;
			else
				_currentState = direction.Y < 0 ? PlayerState.MoveUp : PlayerState.MoveDown;
		}
		else
		{
			Velocity = Velocity.MoveToward(Vector2.Zero, Friction * (float)delta);
			_currentState = PlayerState.Idle;
		}
	}

	private void UpdateAnimation()
	{
		switch (_currentState)
		{
			case PlayerState.Idle:
				_animationPlayer.Play("idle");
				break;
			case PlayerState.MoveLeft:
				_animationPlayer.Play("move_left");
				break;
			case PlayerState.MoveRight:
				_animationPlayer.Play("move_right"); 
				break;
			case PlayerState.MoveUp:
				_animationPlayer.Play("move_up");
				break;
			case PlayerState.MoveDown:
				_animationPlayer.Play("move_down");
				break;
		}
	}
}
