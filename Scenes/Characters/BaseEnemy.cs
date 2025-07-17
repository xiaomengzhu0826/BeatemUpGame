using Godot;
using System;
using System.Collections.Generic;

public partial class BaseEnemy : Character
{
	private const int EDGE_SCREEN_BUFFER = 10;

    [Export] private int _durationAppear;
	[Export] private int _durationBetweenMeleeAttacks;
	[Export] private int _durationPrepMeleeAttack;
	[Export] private int _durationBetweenRangeAttacks;
	[Export] private int _durationPrepRangeAttack;

	public Player Player;

	public int _assignedDoorIndex = -1;
	private EnemySlot _playerSlot = null;
	private float _timeSinceLastMeleeAttack = Time.GetTicksMsec();
	private float _timeSincePrepMeleeAttack = Time.GetTicksMsec();
	private float _timeSinceLastRangeAttack = Time.GetTicksMsec();
	private float _timeSincePrepRangeAttack = Time.GetTicksMsec();
	private float _timeSinceStartAppearing = Time.GetTicksMsec();

	public override void _Ready()
	{
		base._Ready();
		_animationAttacks.AddRange(new string[] { "punch", "punch_alt" });
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		ProcessAppear();
	}

	private void ProcessAppear()
	{
		if (_currentState == State.APPEARING)
		{
			var progress = (Time.GetTicksMsec() - _timeSinceStartAppearing) / _durationAppear;
			if (progress < 1)
			{
				Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, progress);
			}
			else
			{
				Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1);
				_currentState = State.IDLE;
			}
			
		}
	}

	protected override void HandleInput()
	{
		if (Player != null && CanMove())
		{
			if (_canRespawnKnife || _hasKnife || _hasGun)
			{
				GoToRangePosition();
			}
			else
			{
				GoToMeleePosition();
			}
		}
	}

	private void GoToRangePosition()
	{
		var camera = GetViewport().GetCamera2D();
		var screemWidth = GetViewportRect().Size.X;
		var screenLeftEdge = camera.Position.X - screemWidth / 2;
		var screenRightEdge = camera.Position.X + screemWidth / 2;

		var leftDestination = new Vector2(screenLeftEdge + EDGE_SCREEN_BUFFER, Player.Position.Y);
		var rightDestination = new Vector2(screenRightEdge - EDGE_SCREEN_BUFFER, Player.Position.Y);
		var closestDestination = Vector2.Zero;

		if ((leftDestination - Position).Length() < (rightDestination - Position).Length())
		{
			closestDestination = leftDestination;
		}
		else
		{
			closestDestination = rightDestination;
		}

		if ((closestDestination - Position).Length() < 1)
		{
			Velocity = Vector2.Zero;
		}
		else
		{
			Velocity = (closestDestination - Position).Normalized() * _speed;
		}
		if (CanRangeAttack() && _hasKnife && _projectileAim.IsColliding())
		{
			_currentState = State.THROW;
			_timeSinceKnifeDismiss = Time.GetTicksMsec();
			_timeSinceLastRangeAttack = Time.GetTicksMsec();
		}
		if (CanRangeAttack() && _hasGun && _projectileAim.IsColliding())
		{
			_currentState = State.PREP_SHOOT;
			_timeSincePrepRangeAttack = Time.GetTicksMsec();
			_timeSinceLastRangeAttack = Time.GetTicksMsec();
		}

	}

	protected override void HandlePrepShoot()
	{
		if (_currentState == State.PREP_SHOOT && (Time.GetTicksMsec() - _timeSincePrepRangeAttack) > _durationPrepRangeAttack)
		{
			ShootGun();
		}
	}

	public void AssignDoor(Door door)
	{
		if (door._currentState == Door.State.CLOSED)
		{
			_currentState = State.WAIT;
			door.Open();
			door.OnOpen += OnActionCompelete;
		}
		else
		{
			_currentState = State.APPEARING;
			Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0);
			_timeSinceStartAppearing = Time.GetTicksMsec();
		}
	}

	private void GoToMeleePosition()
	{
		if (CanPickupCollectible())
		{
			_currentState = State.PICKUP;
			if (_playerSlot != null)
			{
				Player.FreeSlot(this);
			}
		}
		else if (_playerSlot == null)
		{
			_playerSlot = Player.ReserveSlot(this);
		}

		if (_playerSlot != null)
		{
			var direction = (_playerSlot.GlobalPosition - GlobalPosition).Normalized();

			if (IsPlayerWithinRange())
			{
				Velocity = Vector2.Zero;
				if (CanAttack())
				{
					_currentState = State.PREP_ATTACK;
					_timeSincePrepMeleeAttack = Time.GetTicksMsec();
				}
			}
			else
			{
				Velocity = direction * _speed;
			}
		}
	}

	protected override void HandlePrepAttack()
	{
		if (_currentState == State.PREP_ATTACK && (Time.GetTicksMsec() - _timeSincePrepMeleeAttack) > _durationPrepMeleeAttack)
		{
			_currentState = State.ATTACK;
			_timeSinceLastMeleeAttack = Time.GetTicksMsec();
			_animationAttacks.Shuffle();
		}
	}

	private bool IsPlayerWithinRange()
	{
		return (_playerSlot.GlobalPosition - GlobalPosition).Length() < 1;
	}

	protected override bool CanAttack()
	{
		if ((Time.GetTicksMsec() - _timeSinceLastMeleeAttack) < _durationBetweenMeleeAttacks)
		{
			return false;
		}
		return base.CanAttack();
	}

	protected bool CanRangeAttack()
	{
		if ((Time.GetTicksMsec() - _timeSinceLastRangeAttack) < _durationBetweenRangeAttacks)
		{
			return false;
		}
		return CanAttack();
	}

	protected override void SetHeading()
	{
		if (Player == null || !CanMove())
		{
			return;
		}
		if (GlobalPosition.X > Player.GlobalPosition.X)
		{
			_heading = Vector2.Left;
		}
		else
		{
			_heading = Vector2.Right;
		}
	}

	protected override void OnDamageReceived(int damage, Vector2 direction, DamageReceiver.HitType hitType)
	{
		base.OnDamageReceived(damage, direction, hitType);
		if (_currentHealth == 0)
		{
			Player.FreeSlot(this);
			SignalManager.EmitOnDeathEnemy(this);
		}
	}
}
