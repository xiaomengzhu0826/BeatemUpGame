using Godot;
using System;

public partial class DeathScreen : MarginContainer
{
	[Signal] public delegate void OnGameOverEventHandler();

	[Export] private int _countdownStart;

	private Timer _timer;
	private Label _countdownLabel;

	private int _currentCount;

	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_countdownLabel = GetNode<Label>("%CountdownLabel");

		_currentCount = _countdownStart;

		_timer.Timeout += OnTimerTimeout;

		Refresh();
	}

	public override void _Process(double delta)
	{
		if (_currentCount < _countdownStart && (Input.IsActionJustPressed("attack") || Input.IsActionJustPressed("jump")))
		{
			SignalManager.EmitOnPlayerRevive();
			QueueFree();
		}
	}

	private void Refresh()
	{
		_countdownLabel.Text = _currentCount.ToString();
	}

	private void OnTimerTimeout()
	{
		if (_currentCount > 0)
		{
			_currentCount -= 1;
			Refresh();
		}
		else
		{
			EmitSignal(SignalName.OnGameOver);
			QueueFree();
		}
	}

}
