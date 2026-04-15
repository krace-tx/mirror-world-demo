using Godot;
using MirrorWorldDemo.Script.Core;

namespace MirrorWorldDemo.Scenes.Agent.Skadi;

public partial class Skadi : CharacterBody2D
{
	public const float Speed = 120.0f;

	private AnimatedSprite2D _anim;

	private enum State
	{
		Idle,
		Move,
		Sleep,
		Sound,
		Talk,
		TalkDisagree
	}

	private State _state = State.Idle;

	public override void _Ready()
	{
		SetupPhysics();
		_anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_anim.AnimationFinished += OnAnimationFinished;

		PlayState(State.Idle);
	}

	private void SetupPhysics()
	{
		YSortEnabled = true;
		ZIndex = 0;
		CollisionLayer = (uint)CollisionLayerEnum.Player;
		CollisionMask  = (uint)(CollisionLayerEnum.Wall | CollisionLayerEnum.Object);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsLockedState()) return;

		HandleMovement(delta);
		HandleInput();
	}

	// =========================
	// 🔒 状态锁定判断
	// =========================

	/// <summary>
	/// 这些状态下玩家不能移动也不能输入（如对话中、睡觉中）
	/// </summary>
	private bool IsLockedState()
	{
		return _state == State.Talk
			|| _state == State.TalkDisagree
			|| _state == State.Sleep
			|| _state == State.Sound;
	}

	// =========================
	// 🎮 输入处理
	// =========================
	private void HandleInput()
	{
		// 预留：如有需要可在此添加其他按键逻辑
	}

	// =========================
	// 🚶 移动逻辑
	// =========================
	private void HandleMovement(double delta)
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		if (direction != Vector2.Zero)
		{
			Velocity = direction * Speed;

			_anim.FlipH = direction.X < 0;

			if (_state != State.Move)
				PlayState(State.Move);
		}
		else
		{
			// 减速到零
			Velocity = Velocity.MoveToward(Vector2.Zero, Speed);

			// ✅ 修复5：停止时切回 Idle
			if (_state != State.Idle)
				PlayState(State.Idle);
		}

		MoveAndSlide();
	}

	// =========================
	// 🎬 状态切换 & 动画播放
	// =========================

	private void PlayState(State newState)
	{
		_state = newState;

		// 状态名映射到动画名（AnimatedSprite2D 中需要有对应名称的动画）
		string animName = newState switch
		{
			State.Idle        => "Idle",
			State.Move        => "Move",
			State.Sleep       => "Sleep",
			State.Sound       => "Sound",
			State.Talk        => "Talk",
			State.TalkDisagree=> "TalkDisagree",
			_                 => "Idle"
		};

		// 避免重复播放同一动画
		if (_anim.Animation != animName)
			_anim.Play(animName);
	}

	// =========================
	// 🔔 动画结束回调
	// =========================

	private void OnAnimationFinished()
	{
		switch (_state)
		{
			// Sound / TalkDisagree 播放完后回到 Talk（或根据需求改为 Idle）
			case State.Sound:
			case State.TalkDisagree:
				PlayState(State.Talk);
				break;

			// Sleep 是循环动画，不需要处理结束
			// Talk 也是等待外部调用 EndTalk() 来结束
		}
	}

	// =========================
	// 💬 对话接口（由外部 NPC / 对话系统调用）
	// =========================

	/// <summary>
	/// NPC 触发对话时调用
	/// </summary>
	public void StartTalk()
	{
		PlayState(State.Talk);
	}

	/// <summary>
	/// 玩家选择"不同意"选项时调用
	/// </summary>
	public void OnDisagree()
	{
		PlayState(State.TalkDisagree);
	}

	/// <summary>
	/// 对话结束后调用，恢复正常状态
	/// </summary>
	public void EndTalk()
	{
		PlayState(State.Idle);
	}

	/// <summary>
	/// 触发音效/反应动画（如惊讶、感叹）
	/// </summary>
	public void PlaySound()
	{
		PlayState(State.Sound);
	}

	/// <summary>
	/// 进入睡眠状态
	/// </summary>
	public void StartSleep()
	{
		PlayState(State.Sleep);
	}
}
