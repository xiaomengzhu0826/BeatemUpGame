using Godot;
using System;

public partial class ScoreIndicator : Label
{
	[Export] private float _durationScoreUpdate;

	private int _currentScore;
	private int _displayedScore;
	private int _priorScore;
	private int _realScore;
	private float _timeStartUpdate = Time.GetTicksMsec();

	public override void _Ready()
	{
		_displayedScore = 0;
		Refresh();
	}

	public override void _Process(double delta)
	{
		if (_realScore != _displayedScore)
		{
			var progress = (Time.GetTicksMsec() - _timeStartUpdate) / _durationScoreUpdate;
			if (progress < 1)
			{
				_displayedScore = (int)Mathf.Lerp(_priorScore, _realScore, progress);
			}
			else
			{
				_displayedScore = _realScore;
			}
			Refresh();
		}
	}

	public void AddCombo(int points)
	{
		_realScore += (points * (points + 1) / 2);
		_priorScore = _displayedScore;
		_timeStartUpdate = Time.GetTicksMsec();
		Refresh();
	}

	private void Refresh()
	{
		Text =$"{_displayedScore}";
	}
}
