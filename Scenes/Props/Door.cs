using Godot;
using System;

public partial class Door : Node2D
{
	[Signal] public delegate void OnOpenEventHandler();
	
	[Export] private float _durationOpen;
	[Export] public Godot.Collections.Array<BaseEnemy> _enemies;

	private Sprite2D _doorSprite;

	private float _doorHeight;
	public State _currentState;
	private float _timeStartOpening = Time.GetTicksMsec();

	public enum State
	{
		CLOSED,
		OPENING,
		OPENED
	}

	public override void _Ready()
	{
		_doorSprite = GetNode<Sprite2D>("DoorSprite");

		_doorHeight = _doorSprite.Texture.GetHeight();
	}

	public override void _Process(double delta)
	{
		if (_currentState == State.OPENING)
		{
			if (Time.GetTicksMsec() - _timeStartOpening > _durationOpen)
			{
				_currentState = State.OPENED;
				_doorSprite.Position = Vector2.Up * _doorHeight;
				EmitSignal(SignalName.OnOpen);
			}
			else
			{
				var progress = (Time.GetTicksMsec() - _timeStartOpening) / _durationOpen;
				_doorSprite.Position = new Vector2(_doorSprite.Position.X, Mathf.Lerp(0, -_doorHeight, progress));
			}
		}
	}

	public void Open()
	{
		if (_currentState == State.CLOSED)
		{
			_currentState = State.OPENING;
			_timeStartOpening = Time.GetTicksMsec();
			
		}
	}


}
