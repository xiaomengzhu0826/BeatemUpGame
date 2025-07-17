using Godot;
using System;
using System.Collections.Generic;

public partial class Collectible : Area2D
{
	private const float GRAVITY = 600.0f;

	[Export] private int _damage;
	[Export] private float _speed;
	[Export] private float _knockdownIntensity;
	[Export] private Type _type;
	[Export] public bool _autoDestroy;
	
	private AnimationPlayer _animationPlayer;
	private Sprite2D _collectibleSprite;
	private Area2D _damageEmitter;

	
	public State _currentState ;
	public Type _currentType ;
	public Vector2 _direction = Vector2.Zero;
	protected Dictionary<State, string> _animationMap;
	public float _height = 0;
	protected float _heightSpeed = 0;
	protected Vector2 _velocity = Vector2.Zero;

	public enum State
	{
		FALL,
		GROUNDED,
		FLY
	}

	public enum Type
	{
		KNIFE,
		GUN,
		FOOD
	}

	public override void _Ready()
	{
		NodeInitiation();
		if (_currentState == State.FLY)
		{
			_velocity = _direction * _speed;
		}
		_animationMap = new Dictionary<State, string>()
		{
			{State.FALL,"fall"},
			{State.GROUNDED,"grounded"},
			{State.FLY,"fly"},

		};

		_heightSpeed = _knockdownIntensity;

		_damageEmitter.AreaEntered += OnDamageEmitterAreaEntered;
		_damageEmitter.BodyEntered += OnDamageEmitterBodyEntered;
		_damageEmitter.Position = Vector2.Up * _height;

	}

	private void NodeInitiation()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_collectibleSprite = GetNode<Sprite2D>("CollecibleSprite");
		_damageEmitter = GetNode<Area2D>("DamageEmitter");
	}

	public override void _Process(double delta)
	{
		HandleFall(delta);
		HandleAnimations();
		_collectibleSprite.FlipH = _velocity.X < 0;
		_collectibleSprite.Position = Vector2.Up * _height;
		Position += _velocity * (float)delta;

		if (_currentState == State.GROUNDED)
		{
			Monitorable = true;
			Monitoring = false;
		}
		else
		{
			Monitorable = false;
			Monitoring = true;
		}
		//_damageEmitter.SetDeferred("monitoring", _currentState == State.FLY);
		_damageEmitter.Monitoring = _currentState == State.FLY;
	}

	private void HandleFall(double delta)
	{
		if (_currentState == State.FALL)
		{
			_height += _heightSpeed * (float)delta;
			if (_autoDestroy)
			{
				Modulate=new Color(Modulate.R, Modulate.G, Modulate.B, Modulate.A -(float) delta);
			}
			if (_height < 0)
				{
					_height = 0;
					_currentState = State.GROUNDED;
					if (_autoDestroy)
					{
						QueueFree();
					}
				}
				else
				{
					_heightSpeed -= GRAVITY * (float)delta;
				}
		}
	}

	private void HandleAnimations()
	{
		_animationPlayer.Play(_animationMap[_currentState]);
	}

	private void OnDamageEmitterAreaEntered(Area2D area)
	{
		var damageReceive = (DamageReceiver)area;
		damageReceive.EmitSignal(DamageReceiver.SignalName.OnDamageReceived, _damage, _direction, (int)DamageReceiver.HitType.KNOCKDOWN);
		QueueFree();
	}
	
	private void OnDamageEmitterBodyEntered(Node2D body)
    {
		QueueFree();
    }
}
