using Godot;
using System;

public partial class GameOverScreen : Control
{
	private ScoreIndicator _scoreIndicator;
	private Timer _timer;

	private int _totalScore = 0;

	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_scoreIndicator = GetNode<ScoreIndicator>("%ScoreIndicator");

		_timer.Timeout += OnTimerTimeout;
	}

	public void SetScore(int score)
	{
		_totalScore = score;
	}

    private void OnTimerTimeout()
	{
		_scoreIndicator.AddPoints(_totalScore);
	}

}
