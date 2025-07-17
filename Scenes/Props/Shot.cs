using Godot;
using System;

public partial class Shot : Line2D
{
	[Export] private float _durationShotAcrossScreen;

	private float _durationShot = 0.0f;
	private float _height = 0.0f;
	private float _shotDistance = 0.0f;
	private float _timeStart = Time.GetTicksMsec();

	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{
		var elapsed = Time.GetTicksMsec() - _timeStart;
		var progress = elapsed / _durationShot;
		float newX = Mathf.Lerp(0.0f, _shotDistance, progress);
		SetPointPosition(0, new Vector2(newX, -_height));
		if (progress >= 1)
		{
			QueueFree();
		}
	}

	public void Initialize(float distance, float gunHeight)
	{
		_height = gunHeight;
		_shotDistance = distance;
		AddPoint(new Vector2(0, -_height), 0);
		AddPoint(new Vector2(distance, -_height), 1);
		_durationShot = Mathf.Abs(_shotDistance * _durationShotAcrossScreen / GetViewportRect().Size.X);
	}


}
