using Godot;
using System;

public partial class Player : CharacterBody2D
{
    // 定义信号
    [Signal]
    public delegate void HealthChangedEventHandler(int currentHealth);
    
    [Signal]
    public delegate void PlayerDiedEventHandler();
    
    [Signal]
    public delegate void ScoreIncreasedEventHandler(int newScore);
    
    private int _health = 100;
    private int _score = 0;
    
    public int Health
    {
        get => _health;
        set
        {
            _health = Math.Max(0, value);
            // 发射信号
            EmitSignal(SignalName.HealthChanged, _health);
            
            if (_health <= 0)
            {
                EmitSignal(SignalName.PlayerDied);
            }
        }
    }
    
    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            EmitSignal(SignalName.ScoreIncreased, _score);
        }
    }
    
    public override void _Ready()
    {
        // 可以连接自己的信号
        HealthChanged += OnHealthChanged;
        PlayerDied += OnPlayerDied;
    }
    
    private void OnHealthChanged(int currentHealth)
    {
        GD.Print($"玩家血量变为: {currentHealth}");
    }
    
    private void OnPlayerDied()
    {
        GD.Print("玩家死亡！");
        // 游戏结束逻辑
    }
    
    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
    
    public void AddScore(int points)
    {
        Score += points;
    }
}