using Godot;
using System;

public partial class MainCamera : Camera2D
{
	[Export] private int _durationShake;
	[Export] private float _shakeIntensity;

	private bool _isShaking;
	private float _timeStartShaking = Time.GetTicksMsec();

	public override void _Ready()
	{
		SignalManager.Instance.OnHeavyBlowReceived += OnHeavyBlowReceived;
	}

	public override void _Process(double delta)
	{
		if (_isShaking && (Time.GetTicksMsec() - _timeStartShaking) < _durationShake)
		{
			Offset = new Vector2((float)GD.RandRange(-_shakeIntensity, _shakeIntensity), (float)GD.RandRange(-_shakeIntensity, _shakeIntensity));
		}
		else
		{
			Offset = Vector2.Zero;
			_isShaking = false;
		}
	}

	private void OnHeavyBlowReceived()
	{
		if (OptionsManager.Instance._isScreenShakeEnabled)
		{
			_isShaking = true;
			_timeStartShaking = Time.GetTicksMsec();
		}

	}
}
