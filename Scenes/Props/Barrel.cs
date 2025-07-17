using Godot;
using System;

public partial class Barrel : StaticBody2D
{
	[Export] private float _knockbackIntensity;
	[Export] private Collectible.Type _contentType;

	private const float GRAVITY = 600.0f;

	public DamageReceiver _damageReceiver;
	private Sprite2D _sprite2D;

	private Vector2 _velocity = Vector2.Zero;
	private State _currentState;
	private float _height;
	private float _heightSpeed;

	public enum State
	{
		IDLE,
		DESTROYED
	}

	public override void _Ready()
	{
		_damageReceiver = GetNode<DamageReceiver>("%DamageReceiver");
		_sprite2D = GetNode<Sprite2D>("Sprite2D");

		_currentState = State.IDLE;

		_damageReceiver.OnDamageReceived += OnDamageReceived;
	}

	public override void _Process(double delta)
	{
		Position += _velocity * (float)delta;
		_sprite2D.Position=Vector2.Up*_height;
		HandleAirTime((float)delta);
	}

	private void OnDamageReceived(int damage, Vector2 direction,DamageReceiver.HitType hitType)
	{
		if (_currentState == State.IDLE)
		{
			_sprite2D.Frame = 1;
			_heightSpeed = _knockbackIntensity * 2;
			_currentState = State.DESTROYED;
			_velocity = direction * _knockbackIntensity;
			SignalManager.EmitOnSpawnCollectible((int)_contentType, (int)Collectible.State.FALL, GlobalPosition, Vector2.Zero, 0.0f,false);
		}
		
	}

	private void HandleAirTime(float delta)
	{
		if (_currentState == State.DESTROYED)
		{
			Modulate=new Color(Modulate.R, Modulate.G, Modulate.B, Modulate.A - delta);
			_height += _heightSpeed * delta;
			if (_height < 0)
			{
				_height = 0;
				QueueFree();
			}
			else
			{
				_heightSpeed -=GRAVITY*delta;
			}
		}
	}



}
