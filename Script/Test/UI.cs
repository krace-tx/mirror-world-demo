using Godot;
using System;

public partial class UI : CanvasLayer
{
    private Label _healthLabel;
    private Label _scoreLabel;
    
    public override void _Ready()
    {
        _healthLabel = GetNode<Label>("HealthLabel");
        _scoreLabel = GetNode<Label>("ScoreLabel");
        
        // 获取玩家节点并连接信号
        var player = GetNode<Player>("../Player");
        
        // 方式1: 使用 lambda 表达式
        player.HealthChanged += (currentHealth) => {
            _healthLabel.Text = $"血量: {currentHealth}";
        };
        
        // 方式2: 连接到方法
        player.ScoreIncreased += OnScoreIncreased;
        player.PlayerDied += ShowGameOver;
    }
    
    private void OnScoreIncreased(int newScore)
    {
        _scoreLabel.Text = $"分数: {newScore}";
    }
    
    private void ShowGameOver()
    {
        var gameOverLabel = GetNode<Label>("GameOverLabel");
        gameOverLabel.Visible = true;
        GetTree().CreateTimer(2.0f).Timeout += () => 
        {
            GetTree().ReloadCurrentScene();
        };
    }
}