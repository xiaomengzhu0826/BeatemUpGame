using Godot;
using System;

public partial class ComboIndicator : Label
{
	[Signal] public delegate void OnComboResetEventHandler(int score);

	[Export] private int _durationComboTimeout;

	private float _timeSinceRegisterHit = Time.GetTicksMsec();
	private int _currentCombo;

	public override void _Ready()
	{
		SignalManager.Instance.OnRegisterHit += OnRegisterHit;
		Refresh();
	}

	public override void _Process(double delta)
	{
		if (_currentCombo > 0 && Time.GetTicksMsec() - _timeSinceRegisterHit > _durationComboTimeout)
		{
			EmitSignal(SignalName.OnComboReset, _currentCombo);
			_currentCombo = 0;
			Refresh();
		}
	}

	private void OnRegisterHit()
	{
		_currentCombo += 1;
		_timeSinceRegisterHit = Time.GetTicksMsec();
		Refresh();
	}

	private void Refresh()
	{
		Text = $"x{_currentCombo}";
		Visible = _currentCombo>0;
	}

}
