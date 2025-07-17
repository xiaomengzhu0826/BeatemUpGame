using Godot;
using System;
using System.Collections.Generic;

public partial class IgorBoss : Character
{
	private const int GROUND_FRICTION = 50;

	[Export] private Player _player;
	[Export] private int _durationBetweenAttacks;
	[Export] private int _durationVulnerable;
	[Export] private int _distanceFromPlayer;

	private Vector2 _knockbackForce = Vector2.Zero;
	private float _timeLastAttack = Time.GetTicksMsec();
	private float _timeStartVulnerable = Time.GetTicksMsec();

	public override void _Process(double delta)
	{
		base._Process(delta);
		_knockbackForce = _knockbackForce.MoveToward(Vector2.Zero, (float)delta * GROUND_FRICTION);
	}

	private Vector2 GetTargetDestination()
	{
		var target = Vector2.Zero;
		if (Position.X < _player.Position.X)
		{
			target = _player.Position + Vector2.Left * _distanceFromPlayer;
		}
		else
		{
			target = _player.Position + Vector2.Right * _distanceFromPlayer;
		}
		return target;
	}

	private bool IsPlayerWithinRange()
	{
		var target = GetTargetDestination();
		return (target - Position).Length() < 1;
	}

	protected override void HandleInput()
	{
		if (_player != null && CanMove())
		{
			if (CanAttack() && _projectileAim.IsColliding())
			{
				_currentState = State.FLY;
				Velocity = _heading * _flightSpeed;
			}
			else
			{
				if (IsPlayerWithinRange())
				{
					Velocity = Vector2.Zero;
					_currentState = State.IDLE;
				}
				else
				{
					var targetDestination = GetTargetDestination();
					var direction = (targetDestination - Position).Normalized();
					Velocity = (direction + _knockbackForce) * _speed;
					_currentState = State.WALK;
				}
			}

		}
	}

	protected override void SetHeading()
	{
		if (_player == null || !CanMove())
		{
			return;
		}
		_heading = Position.X > _player.Position.X ? Vector2.Left : Vector2.Right;
	}

	protected override bool CanGetHurt()
	{
		return true;
	}

	protected override bool CanAttack()
	{
		if (Time.GetTicksMsec() - _timeLastAttack < _durationBetweenAttacks)
		{
			return false;
		}
		return base.CanAttack();
	}


	private bool IsVulnerable()
	{
		return _currentState == State.RECOVERY;
	}

	protected override void OnDamageReceived(int damage, Vector2 direction, DamageReceiver.HitType hitType)
	{
		if (!IsVulnerable())
		{
			_knockbackForce = direction * _knockbackIntensity;
			return;
		}
		_currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
		if (_currentHealth == 0)
		{
			_currentState = State.FALL;
			_heightSpeed = _knockdownIntensity;
			Velocity = direction * _knockdownIntensity;
			SignalManager.EmitOnDeathEnemy(this);
		}
		else
		{
			Velocity = Vector2.Zero;
			_currentState = State.HURT;
		}
	}

	protected override void HandleGrounded()
	{
		if (_currentState == State.GROUNDED && _currentHealth > 0)
		{
			_currentState = State.RECOVERY;
			_timeStartVulnerable = Time.GetTicksMsec();
		}
		else if (_currentState == State.RECOVERY && Time.GetTicksMsec() - _timeStartVulnerable > _durationVulnerable)
		{
			_currentState = State.IDLE;
			_timeLastAttack = Time.GetTicksMsec();
		}
	}

	public override void OnActionCompelete()
	{
		if (_currentState == State.HURT)
		{
			_currentState = State.RECOVERY;
			return;
		}
		base.OnActionCompelete();
	}

	protected override void OnDamageEmit(Area2D area)
	{
		var damageReciver = (DamageReceiver)area;
		damageReciver.EmitSignal(DamageReceiver.SignalName.OnDamageReceived, _damage, _heading, (int)DamageReceiver.HitType.KNOCKDOWN);
		_currentState = State.IDLE;
	}
	
	protected override bool IsAttacking()
	{
		return _currentState == State.FLY;
	}
}
